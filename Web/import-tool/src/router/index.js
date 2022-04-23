import { createRouter, createWebHistory } from "vue-router";

const router = createRouter({
	history: createWebHistory(),
	routes: [
		{
			path: "/",
			name: "Home",
			component: () => import("../views/Home.vue")
		},
		{
			path: "/signup",
			name: "Sign up",
			component: () => import("../views/SignUp.vue")
		},
		{
			path: "/login",
			name: "Login",
			component: () => import("../views/LogIn.vue")
		},
		{
			path: "/tool",
			name: "Tool",
			component: () => import("../views/Tool.vue"),
			meta: {
				requireAuth: true
			}
		},
	],
});

export default router;