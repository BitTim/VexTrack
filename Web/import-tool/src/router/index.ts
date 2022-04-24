import { createRouter, createWebHistory } from "vue-router"
import { auth } from "../firebase"

import Home from "../views/Home.vue"
import Login from "../views/Login.vue"
import SignUp from "../views/SignUp.vue"

const routes = [
	{
		path: "/",
		name: "Home",
		component: Home,
		meta:
		{
			requiresAuth: true,
		},
	},
	{
		path: "/login",
		name: "Login",
		component: Login,
		meta:
		{
			requiresAuth: false,
		},
	},
	{
		path: "/signup",
		name: "SignUp",
		component: SignUp,
		meta:
		{
			requiresAuth: false,
		},
	},
]

const router = createRouter({
	history: createWebHistory(),
	routes
})

router.beforeEach((to, from, next) => {
	if(to.matched.some(record => !record.meta.requiresAuth) && auth.currentUser)
	{
		next("/");
		return;
	}

	if(to.matched.some(record => record.meta.requiresAuth) && !auth.currentUser)
	{
		next("/login")
		return;
	}

	next();
})

export default router
