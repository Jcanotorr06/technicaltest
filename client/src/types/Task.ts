export type Task = {
	id: string;
	title: string;
	description: string;
	dueDate: Date;
	status: "Pending" | "In Progress" | "Completed";
	statusId: number;
	listId: string;
	createdBy: string;
	assignedTo: string;
	order: number;
	tags: [];
};

export type CreateTaskRequest = Omit<
	Task,
	"id" | "createdBy" | "order" | "tags" | "statusId" | "status"
> & {
	status: number;
};
