<template>
	<div>
		<h1>Login</h1>
		<form @submit.prevent="login">
			<input type="email" placeholder="E-Mail" v-model="email" />
			<input type="password" placeholder="Password" v-model="password" />
			<input type="submit" value="Login" />
		</form>
	</div>
</template>

<script lang="ts">
import { ref } from "vue"
import { useUserStore } from "../store"

export default {
	setup: () => {
		const email = ref("")
		const password = ref("")
		const store = useUserStore()

		const login = () => {
			store.login(email.value, password.value);
		}

		return { email, password, login }
	}
}

</script>