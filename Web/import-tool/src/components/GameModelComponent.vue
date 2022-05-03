<template>
	<details>
		<summary>{{ mode }} {{ score }} - {{ enemyScore }}</summary>

		<p>{{ new Date(time * 1000).toString() }}</p>
		<p>{{ desc }}</p>
		<p>{{ map }}</p>
		<p>{{ xp }} XP</p>
		<p>Surrendered Win: {{ surrenderedWin }}</p>
		<p>Surrendered Loss: {{ surrenderedLoss }}</p>
		<br>
	</details>
</template>

<script lang="ts">
export default {
	props: {
		mode: {
			type: String,
			required: true,
			default: "",
		},
		time: {
			type: Number,
			required: true,
			default: 0,
		},
		xp: {
			type: Number,
			required: true,
			default: 0,
		},
		map: {
			type: String,
			required: true,
			default: "",
		},
		desc: {
			type: String,
			required: true,
			default: "",
		},
		score: {
			type: Number,
			required: true,
			default: 0,
		},
		enemyScore: {
			type: Number,
			required: true,
			default: 0,
		},
		surrenderedWin: {
			type: Boolean,
			required: true,
			default: false,
		},
		surrenderedLoss: {
			type: Boolean,
			required: true,
			default: false,
		},
	}
}
</script>

<style>
summary {
	font-weight: bold;
}
</style>