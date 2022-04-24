import { createRouter, createWebHistory } from "vue-router"

const routes = [
	{
		path: "/",
		name: "Home",
		component: () => import("../views/Home.vue"),
		meta:
		{
			requiresAuth: true,
		},
	},
	{
		path: "/login",
		name: "Login",
		component: () => import("../views/Login.vue"),
	},
	{
		path: "/signup",
		name: "Sign up",
		component: () => import("../views/SignUp.vue"),
	},
]

const router = createRouter({
	history: createWebHistory(),
	routes
})

export default router
