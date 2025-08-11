export type Task = {
	id: string;
	title: string;
	description: string;
	dueDate: Date;
	status: "Pending" | "In Progress" | "Completed";
	statusId: number;
	listId: string;
	listName?: string;
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

export type UpdateTaskRequest = Omit<
	Task,
	"createdBy" | "tags" | "statusId" | "status"
> & {
	status: number;
};
