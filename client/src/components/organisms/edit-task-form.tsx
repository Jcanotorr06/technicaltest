import type { FC } from "react";
import { DialogContent, Form } from "@/components/atoms";
import type { Task } from "@/types";
import z from "zod";
import { Status } from "@/lib/const";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";

type Props = {
	task: Task;
};

const today = new Date();
const yesterday = new Date(today);
yesterday.setDate(today.getDate() - 1);

const formSchema = z.object({
	title: z
		.string()
		.min(2, {
			error: "Title must be at least 2 characters long",
		})
		.max(100, {
			error: "Title cannot exceed 100 characters",
		}),
	description: z
		.string()
		.max(500, {
			error: "Description cannot exceed 500 characters",
		})
		.optional(),
	dueDate: z.date().min(yesterday, {
		error: "Due date must be today or in the future",
	}),
	listId: z.uuid({
		error: "List ID is required",
	}),
	statusId: z.number().min(Status.PENDING.id).max(Status.COMPLETED.id),
});

type FormSchemaType = z.infer<typeof formSchema>;

const EditTaskForm: FC<Props> = (props) => {
	const { task } = props;

	const form = useForm<FormSchemaType>({
		resolver: zodResolver(formSchema),
		defaultValues: {
			title: task.title,
			description: task.description,
			dueDate: task.dueDate,
			listId: task.listId,
			statusId: task.statusId,
		},
	});

	const onSubmit = async (data: FormSchemaType) => {
		console.log("Form submitted with data:", data);
	};
	return (
		<DialogContent>
			<Form {...form}>
				<form
					onSubmit={form.handleSubmit(onSubmit)}
					className="space-y-4"
				></form>
			</Form>
		</DialogContent>
	);
};

EditTaskForm.displayName = "EditTaskForm";

export default EditTaskForm;
