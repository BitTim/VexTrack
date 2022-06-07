<template>
	<div>
		<h1>Login</h1>
		<form @submit.prevent="login">
			<input type="email" placeholder="E-Mail" v-model="email" />
			<input type="password" placeholder="Password" v-model="password" />
			<input type="submit" value="Login" />
		</form>
		<button @click="routeSignUp">Need an account?</button>
	</div>
</template>

<script lang="ts">
import { defineComponent, ref } from "vue"
import { useUserStore } from "../store"
import router from "../router"

export default defineComponent({
	setup: () => {
		const email = ref("")
		const password = ref("")
		const store = useUserStore()

		const login = () => {
			store.login(email.value, password.value);
		}

		const routeSignUp = () => {
			router.push("/signup")
		}

		return { email, password, login, routeSignUp }
	}
})

</script>