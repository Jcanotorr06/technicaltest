export const Status = {
	PENDING: {
		id: 1,
		label: "Pending",
		variant: "warning",
	},
	IN_PROGRESS: {
		id: 2,
		label: "In Progress",
		variant: "info",
	},
	COMPLETED: {
		id: 3,
		label: "Completed",
		variant: "success",
	},
} as const;
