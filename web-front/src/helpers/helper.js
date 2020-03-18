import axios from 'axios'
import Cookies from 'js-cookie'

export function bytesToSize(bytes) {
    if (bytes === 0) return '0 B';
    var k = 1024, // or 1024
        sizes = ['B', 'KB', 'MB', 'GB', 'TB', 'PB', 'EB', 'ZB', 'YB'],
        i = Math.floor(Math.log(bytes) / Math.log(k));
    return (bytes / Math.pow(k, i)).toPrecision(3) + ' ' + sizes[i];
}

export function postWithToken(url, data, callback) {
    axios({
        url: url,
        method: 'post',
        data: data,
        headers: {
            'Authorization': `Bearer ${Cookies.get('token')}`
        }
    }).then(callback)
}

export function getWithToken(url, data, callback) {
    axios({
        url: url,
        method: 'get',
        data: data,
        headers: {
            'Authorization': `Bearer ${Cookies.get('token')}`
        }
    }).then(callback)
}