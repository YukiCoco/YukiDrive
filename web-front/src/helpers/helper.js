import axios from 'axios'
import Cookies from 'js-cookie'
import Vuex from '../store/index'

//文件大小转换
export function bytesToSize(bytes) {
    if (bytes === 0) return '0 B';
    var k = 1024, // or 1024
        sizes = ['B', 'KB', 'MB', 'GB', 'TB', 'PB', 'EB', 'ZB', 'YB'],
        i = Math.floor(Math.log(bytes) / Math.log(k));
    return (bytes / Math.pow(k, i)).toPrecision(3) + ' ' + sizes[i];
}

let errorCallback = function (error) {
    Vuex.commit('openSnackBar', '出现错误：' + error.response.data.message)
}

export function postWithToken(url, data, callback) {
    axios({
        url: url,
        method: 'post',
        data: data,
        headers: {
            'Authorization': `Bearer ${Cookies.get('token')}`
        }
    }).then(callback).catch(errorCallback)
}

export function getWithToken(url, data, callback) {
    axios({
        url: url,
        method: 'get',
        data: data,
        headers: {
            'Authorization': `Bearer ${Cookies.get('token')}`
        }
    }).then(callback).catch(errorCallback)
}

export function deleteWithToken(url, data, callback) {
    axios({
        url: url,
        method: 'delete',
        params: data,
        headers: {
            'Authorization': `Bearer ${Cookies.get('token')}`
        }
    }).then(callback).catch(errorCallback)
}