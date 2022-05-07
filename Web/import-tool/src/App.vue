<template>
	<div class="bg-lightBG text-lightFG dark:bg-DarkBG dark:text-DarkFG min-h-screen p-2">
		<nav class="flex flex-wrap 
			bg-LightShade dark:bg-DarkShade 
			items-center justify-start
			h-16 pl-4 pr-4 p-2 mb-4
			shadow-lg rounded-lg">
			<img class="h-full" src="./assets/logo.png"/>
			<h1 class="font-bold text-xl pl-4">VexTrack Import Tool</h1>

			<router-link class="pl-10" to="/" v-if="store.user">Home</router-link>
			<router-link class="pl-10" to="/login" v-if="!store.user">Login</router-link>
			<router-link class="pl-10" to="/signup" v-if="!store.user">Sign Up</router-link>

			<div class="ml-auto">
				<button v-if="store.user" @click="store.signout()">Logout</button>
			</div>
		</nav>
		
		<router-view/>
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