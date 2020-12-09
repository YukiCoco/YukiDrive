## 使用 CDN 加速下载

### 准备
首先在您的 CDN 供应商中回源地址填写为您实际下载文件文件域名，比如我的为 `yukistudio-my.sharepoint.com`，然后您的 CDN 提供商会为您提供 CDN 的加速域名，如 `ancient-wind-69a2.yukinococo.workers.dev`，记录这个域名

### 配置
打开 `appsettings.json` 文件，在 `CDNUrls` 字段中添加 `实际下载域名;CDN 加速域名`，如我的就填写为 `yukistudio-my.sharepoint.com;ancient-wind-69a2.yukinococo.workers.dev`，然后重启 YukiDrive 即可.

### 高级应用
您可能已经注意到了，`CDNUrls` 是一个 Array，这意味着您可以填写多个下载域名和多个加速域名。因为 onedrive 和 SharePoint 的下载域名通常是不一样的，所以添加多个 CDN 域名是必要的.  
比如我的最终就填写为
```` json
"CDNUrls" : [
      "yukistudio-my.sharepoint.com;ancient-wind-69a2.yukinococo.workers.dev",
      "yukistudio.sharepoint.com;tiny-breeze-28d1.yukinococo.workers.dev"
]
````

### 白嫖 CloudFlare
您可能还注意到了，上文中我的域名是 `*.workers.dev`，这是来自 CloudFlare 的免费反代 CDN，走的都是 CloudFlare 边缘节点，速度还是可以的.

使用 CloudFlare 的 Worker 反代，免费用户每天有 `100,000` 个请求上限，自用完全足够，具体如何开启 Worker 这里不在赘述.

下面就是我目前使用的 Worker 反代脚本，您只需稍作修改就可以使用.

````JavaScript
// Website you intended to retrieve for users.
const upstream = 'yukistudio-my.sharepoint.com' #修改为您的下载域名

// Custom pathname for the upstream website.
const upstream_path = '/'

// Website you intended to retrieve for users using mobile devices.
const upstream_mobile = 'yukistudio-my.sharepoint.com' #修改为您的下载域名

// Countries and regions where you wish to suspend your service.
const blocked_region = []

// IP addresses which you wish to block from using your service.
const blocked_ip_address = ['0.0.0.0', '127.0.0.1']

// Whether to use HTTPS protocol for upstream address.
const https = true

// Whether to disable cache.
const disable_cache = false

// Replace texts.
const replace_dict = {
    '$upstream': '$custom_domain',
    '//sunpma.com': ''
}

addEventListener('fetch', event => {
    event.respondWith(fetchAndApply(event.request));
})

async function fetchAndApply(request) {
    const region = request.headers.get('cf-ipcountry').toUpperCase();
    const ip_address = request.headers.get('cf-connecting-ip');
    const user_agent = request.headers.get('user-agent');

    let response = null;
    let url = new URL(request.url);
    let url_hostname = url.hostname;

    if (https == true) {
        url.protocol = 'https:';
    } else {
        url.protocol = 'http:';
    }

    if (await device_status(user_agent)) {
        var upstream_domain = upstream;
    } else {
        var upstream_domain = upstream_mobile;
    }

    url.host = upstream_domain;
    if (url.pathname == '/') {
        url.pathname = upstream_path;
    } else {
        url.pathname = upstream_path + url.pathname;
    }

    if (blocked_region.includes(region)) {
        response = new Response('Access denied: WorkersProxy is not available in your region yet.', {
            status: 403
        });
    } else if (blocked_ip_address.includes(ip_address)) {
        response = new Response('Access denied: Your IP address is blocked by WorkersProxy.', {
            status: 403
        });
    } else {
        let method = request.method;
        let request_headers = request.headers;
        let new_request_headers = new Headers(request_headers);

        new_request_headers.set('Host', upstream_domain);
        new_request_headers.set('Referer', url.protocol + '//' + url_hostname);

        let original_response = await fetch(url.href, {
            method: method,
            headers: new_request_headers
        })

        connection_upgrade = new_request_headers.get("Upgrade");
        if (connection_upgrade && connection_upgrade.toLowerCase() == "websocket") {
            return original_response;
        }

        let original_response_clone = original_response.clone();
        let original_text = null;
        let response_headers = original_response.headers;
        let new_response_headers = new Headers(response_headers);
        let status = original_response.status;
        
        if (disable_cache) {
            new_response_headers.set('Cache-Control', 'no-store');
        }

        new_response_headers.set('access-control-allow-origin', '*');
        new_response_headers.set('access-control-allow-credentials', true);
        new_response_headers.delete('content-security-policy');
        new_response_headers.delete('content-security-policy-report-only');
        new_response_headers.delete('clear-site-data');
        
        if (new_response_headers.get("x-pjax-url")) {
            new_response_headers.set("x-pjax-url", response_headers.get("x-pjax-url").replace("//" + upstream_domain, "//" + url_hostname));
        }
        
        const content_type = new_response_headers.get('content-type');
        if (content_type != null && content_type.includes('text/html') && content_type.includes('UTF-8')) {
            original_text = await replace_response_text(original_response_clone, upstream_domain, url_hostname);
        } else {
            original_text = original_response_clone.body
        }
        
        response = new Response(original_text, {
            status,
            headers: new_response_headers
        })
    }
    return response;
}

async function replace_response_text(response, upstream_domain, host_name) {
    let text = await response.text()

    var i, j;
    for (i in replace_dict) {
        j = replace_dict[i]
        if (i == '$upstream') {
            i = upstream_domain
        } else if (i == '$custom_domain') {
            i = host_name
        }

        if (j == '$upstream') {
            j = upstream_domain
        } else if (j == '$custom_domain') {
            j = host_name
        }

        let re = new RegExp(i, 'g')
        text = text.replace(re, j);
    }
    return text;
}


async function device_status(user_agent_info) {
    var agents = ["Android", "iPhone", "SymbianOS", "Windows Phone", "iPad", "iPod"];
    var flag = true;
    for (var v = 0; v < agents.length; v++) {
        if (user_agent_info.indexOf(agents[v]) > 0) {
            flag = false;
            break;
        }
    }
    return flag;
}
````
