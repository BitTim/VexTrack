<template>
	<div>
		<h1>VexTrack Import Tool</h1>

		<nav>
			<router-link to="/" v-if="store.user">Home</router-link> |
			<router-link to="/login">Login</router-link> |
			<router-link to="/signup">Sign Up</router-link> |
			<button>Logout</button>
		</nav>
		
		<router-view />
	</div>
</template>

<script lang="ts">
import { onBeforeMount } from "vue"
import { useUserStore } from "./store"

export default {
	setup: () => {
		const store = useUserStore();

		onBeforeMount(() => {
			store.fetchUser();
		});

		return { store }
	}
}
</script>

<style>

</style>
