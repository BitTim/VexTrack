<template>
	<div>
		<h1>Create an account</h1>
		<form>
			<input type="text" placeholder="Username" v-model="form.username"/>
			<input type="email" placeholder="E-Mail" v-model="form.email" />
			<input type="password" placeholder="Password" v-model="form.password" />
			<input type="submit" value="signup" />
		</form>
	</div>
</template>

<script>
import { ref } from "vue"
import { useRouter } from "vue-router"
// import { useUserStore } from "../stores"

export default {
	setup () {
		const form = ref({});
		// const store = useUserStore();

		return { form }
	}
}

</script>