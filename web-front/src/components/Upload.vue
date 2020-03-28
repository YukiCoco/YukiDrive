<template>
<div>
    <v-row>
        <v-col cols="12" sm="12" md="8" offset-md="2" align="start" justify="center">
            <v-card>
                <v-card-title primary-title>
                    上传文件
                </v-card-title>
                <v-card-text>
                    <v-select :items="drives" label="选择驱动器" v-model="selectedDrive"></v-select>
                    <file-pond :server="server" allow-revert="false"  instantUpload="true" label-idle="拖拽文件到这里 或者 浏览文件" allow-multiple="true"/>
                    <v-text-field class="mt-4" hide-details readonly label="下载链接" v-for="(item, index) in downloadUrls" :key="index" :value="item"></v-text-field>
                </v-card-text>
            </v-card>
        </v-col>
    </v-row>
</div>
</template>

<script>
// Import Vue FilePond
import vueFilePond from 'vue-filepond';
// Import FilePond styles
import 'filepond/dist/filepond.min.css';
// Import FilePond plugins
// Please note that you need to install these plugins separately
// Import image preview plugin styles
// import 'filepond-plugin-image-preview/dist/filepond-plugin-image-preview.min.css';
// // Import image preview and file type validation plugins
import FilePondPluginFileValidateType from 'filepond-plugin-file-validate-type';
import axios from 'axios'
const FilePond = vueFilePond(FilePondPluginFileValidateType);
// Create component
export default {
    data() {
        return {
            uploadFiles: undefined,
            drives: [],
            downloadUrls: [],
            selectedDrive: undefined,
            //FilePond 文件上传
            server: {
                process: (fieldName, file, metadata, load, error, progress, abort, transfer, options) => {
                    if(this.selectedDrive == undefined) {
                        abort()
                        return
                    }
                    //获取上传 url
                    axios.get(`${this.$store.state.settings.baseUrl}/api/upload/${this.selectedDrive}/${file.name}`).then(response => {
                        let requestUrl = response.data.requestUrl
                        this.downloadUrls.push(response.data.fileUrl)
                        const request = new XMLHttpRequest()
                        request.open('PUT', requestUrl)
                        request.setRequestHeader('Content-Range', `bytes 0-${file.size - 1}/${file.size}`)
                        // Should call the progress method to update the progress to 100% before calling load
                        // Setting computable to false switches the loading indicator to infinite mode
                        request.upload.onprogress = (e) => {
                            progress(e.lengthComputable, e.loaded, e.total)
                        };
                        // Should call the load method when done and pass the returned server file id
                        // this server file id is then used later on when reverting or restoring a file
                        // so your server knows which file to return without exposing that info to the client
                        request.onload = function () {
                            if (request.status >= 200 && request.status < 300) {
                                // the load method accepts either a string (id) or an object
                                load(request.responseText)
                            } else {
                                // Can call the error method if something is wrong, should exit after
                                error('上传错误')
                            }
                        };
                        request.send(file);

                        // Should expose an abort method so the request can be cancelled
                        return {
                            abort: () => {
                                // This function is entered if the user has tapped the cancel button
                                request.abort();
                                // Let FilePond know the request has been cancelled
                                abort();
                            }
                        }
                    })
                }
            },
            allowUpload: false
        }
    },
    mounted() {
        axios.get(this.$store.state.settings.baseUrl + "/api/sites").then(response => {
            response.data.forEach(element => {
                this.drives.push({
                    text: element.nickName,
                    value: element.name
                })
            });
        })
    },
    methods: {
    },
    components: {
        FilePond
    }
}
</script>

<style>

</style>
