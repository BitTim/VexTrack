<template>
	<div>
		<h1>Home</h1>
		<form>
			<p>Please select your data.json here. It is located in %localappdata%/VexTrack/data.json</p>
			<input ref="dataFile" type="file" @change="fileChanged()" />
			<input type="submit" @click="" />
		</form>
	</div>
</template>

<script lang="ts">
import { ref } from "vue";
import { loadFile, parseData } from "../core"

export default {
	setup: () => {
		const dataFile = ref(null);

		const fileChanged = async () => {
			loadFile(dataFile)
		}

		return { dataFile, fileChanged }
	}
}
</script>