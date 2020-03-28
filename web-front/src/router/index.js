import Vue from 'vue'
import VueRouter from 'vue-router'
import Index from '../components/Index'
import Admin from '../components/Admin'
import Login from '../components/Login'
import Show from '../components/Show'
import Upload from '../components/Upload'

Vue.use(VueRouter)

const routes = [
  {
    path: '/show',
    name: 'show',
    component: Show
  },
  {
    path: '/upload',
    name: 'upload',
    component: Upload
  },
  {
    path: '/admin',
    name: 'admin',
    component: Admin
  },
  {
    path: '/login',
    name: 'login',
    component: Login
  },
  {
    path: '/:siteName?/:folderPath*', //Zero or more
    name: 'Index',
    component: Index
  }
]

const router = new VueRouter({
  routes
})

export default router
