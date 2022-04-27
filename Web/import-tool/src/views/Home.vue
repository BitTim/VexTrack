<template>
	<div>
		<h1>Home</h1>
		<p>Please select your data.json here. It is located in %localappdata%/VexTrack/data.json</p>
		<input type="file" accept=".json" @change="fileChanged" />
		<button @click.prevent="importClicked">Import</button>
	</div>
</template>

<script lang="ts">
import { ref } from "vue";
import { loadFile, parsePreview, parseData } from "../core"

export default {
	setup: () => {
		const fileChanged = async (event: any) => {
			loadFile(event?.target?.files[0]);
			parsePreview();
		}

		const importClicked = async () => {
			parseData()
		}

		return { fileChanged, importClicked }
	}
}
</script>