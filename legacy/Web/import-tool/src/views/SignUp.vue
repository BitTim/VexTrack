<template>
	<div>
		<h1>Sign up</h1>
		<form @submit.prevent="signup">
			<input type="text" placeholder="Username" v-model="username" />
			<input type="email" placeholder="E-Mail" v-model="email" />
			<input type="password" placeholder="Password" v-model="password" />
			<input type="submit" value="Login" />
		</form>
		<button @click="routeLogin">Already have an account?</button>
	</div>
</template>

<script lang="ts">
import { defineComponent, ref } from "vue"
import { useUserStore } from "../store"
import router from "../router"

export default defineComponent({
	setup: () => {
		const username = ref("");
		const email = ref("");
		const password = ref("");
		const store = useUserStore()

		const signup = () => {
			store.signup(username.value, email.value, password.value)
		}

		const routeLogin = () => {
			router.push("/login")
		}

		return { username, email, password, signup, routeLogin}
	}
})
</script>