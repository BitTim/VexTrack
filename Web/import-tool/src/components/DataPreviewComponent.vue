<template>
	<div>
		<h4>Preview</h4>
		<h5>{{ preview.season_name }}</h5>
	</div>
</template>

<script lang="ts">
import { GameModel } from "../models/GameModel"

export default {
	data () {
		return {
			season_endDate: String,
			season_activeLevel: Number,
			season_activeXP: Number,
			season_history: [] as GameModel[],
		}
	},
	props: [
		"preview"
	]
}
</script>