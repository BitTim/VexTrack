<template>
	<div>
		<h4>Preview</h4>
		<h5>{{ season_name }}</h5>
	</div>
</template>

<script lang="ts">
import { defaultPreview } from "../core"
import { GameModel } from "../models/GameModel"

export default {
	props: {
		season_name: {
			type: String,
			required: true,
			default: defaultPreview().season_name,
		},
		season_endDate: {
			type: String,
			required: true,
			default: defaultPreview().season_endDate,
		},
		season_activeLevel: {
			type: Number,
			required: true,
			default: defaultPreview().season_activeLevel,
		},
		season_activeXP: {
			type: Number,
			required: true,
			default: defaultPreview().season_activeXP,
		},
		season_history: {
			type: [] as GameModel[],
			required: true,
			default: defaultPreview().season_history,
		}
	}
}
</script>