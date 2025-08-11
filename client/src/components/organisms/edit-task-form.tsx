import {
	Button,
	DialogClose,
	DialogContent,
	DialogFooter,
	DialogHeader,
	DialogTitle,
	Form,
	Input,
	Popover,
	PopoverContent,
	PopoverTrigger,
	Select,
	SelectContent,
	SelectItem,
	SelectTrigger,
	SelectValue,
	Textarea,
} from "@/components/atoms";
import {
	FormControl,
	FormDescription,
	FormField,
	FormItem,
	FormLabel,
	FormMessage,
} from "@/components/atoms/form";
import {
	Command,
	CommandEmpty,
	CommandGroup,
	CommandInput,
	CommandItem,
	CommandList,
	DatePicker,
} from "@/components/molecules";
import { useToken } from "@/context";
import { Status } from "@/lib/const";
import { cn } from "@/lib/utils";
import {
	getListsQueryOptions,
	getPublicListsQueryOptions,
	useUpdateTask,
} from "@/services";
import type { Task, UpdateTaskRequest } from "@/types";
import { zodResolver } from "@hookform/resolvers/zod";
import { useSuspenseQuery } from "@tanstack/react-query";
import { ChevronDown } from "lucide-react";
import type { FC } from "react";
import { useForm } from "react-hook-form";
import { z } from "zod";

type Props = {
	task: Task;
	handleClose: () => void;
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
	statusId: z
		.number()
		.min(Status.PENDING.id, {
			message: "Status must be Pending, In Progress, or Completed",
		})
		.max(Status.COMPLETED.id, {
			message: "Status must be Pending, In Progress, or Completed",
		}),
});

type FormSchemaType = z.infer<typeof formSchema>;

/**
 * EditTaskForm component
 * @param {object} props - The props for the component
 * @param {Task} props.task - The task to be edited
 * @returns {JSX.Element}
 *
 * @example
 * <Dialog>
 * 	<EditTaskForm task={task} />
 * </Dialog>
 */

const EditTaskForm: FC<Props> = (props) => {
	const { task, handleClose } = props;
	const { headers } = useToken();

	const updateTask = useUpdateTask();

	const form = useForm<FormSchemaType>({
		resolver: zodResolver(formSchema),
		defaultValues: {
			title: task.title,
			description: task.description,
			dueDate: task.dueDate ? new Date(task.dueDate) : new Date(),
			listId: task.listId,
			statusId: task.statusId,
		},
	});

	const myListsQuery = useSuspenseQuery(getListsQueryOptions(headers));
	const publicListsQuery = useSuspenseQuery(
		getPublicListsQueryOptions(headers),
	);

	const myLists = myListsQuery.data;
	const publicLists = publicListsQuery.data;

	const availableLists = myLists.concat(
		publicLists.filter(
			(list) => !myLists.some((myList) => myList.id === list.id),
		),
	);

	const onSubmit = async (data: FormSchemaType) => {
		if (!form.formState.isDirty) {
			handleClose();
			return;
		}

		const payload = {
			id: task.id,
			assignedTo: task.assignedTo,
			order: task.order,
			title: data.title,
			description: data.description || "",
			dueDate: data.dueDate,
			listId: data.listId,
			status: data.statusId,
		} satisfies UpdateTaskRequest;

		const response = await updateTask.mutateAsync(payload);

		if (response.id) {
			handleClose();
		}
	};

	return (
		<DialogContent className="sm:max-w-md">
			<DialogHeader>
				<DialogTitle>Edit Task</DialogTitle>
			</DialogHeader>
			<Form {...form}>
				<form onSubmit={form.handleSubmit(onSubmit)} className="space-y-4">
					<FormField
						control={form.control}
						name="title"
						render={({ field }) => (
							<FormItem>
								<FormLabel>Title</FormLabel>
								<FormControl>
									<Input placeholder="Task title" {...field} />
								</FormControl>
								<FormDescription>Enter a title for your task</FormDescription>
								<FormMessage />
							</FormItem>
						)}
					/>
					<FormField
						control={form.control}
						name="description"
						render={({ field }) => (
							<FormItem>
								<FormLabel>Description</FormLabel>
								<FormControl>
									<Textarea
										className="min-h-[80px]"
										placeholder="Task description (optional)"
										{...field}
									/>
								</FormControl>
								<FormDescription>Describe your task</FormDescription>
								<FormMessage />
							</FormItem>
						)}
					/>
					<FormField
						control={form.control}
						name="dueDate"
						render={({ field }) => (
							<FormItem>
								<FormLabel>Due Date</FormLabel>
								<FormControl>
									<DatePicker
										date={field.value}
										onChange={field.onChange}
										className="justify-start"
									/>
								</FormControl>
								<FormDescription>Pick a due date</FormDescription>
								<FormMessage />
							</FormItem>
						)}
					/>
					<FormField
						control={form.control}
						name="listId"
						render={({ field }) => (
							<FormItem>
								<FormLabel>List</FormLabel>
								<Popover>
									<PopoverTrigger asChild>
										<FormControl>
											<Button
												variant="outline"
												role="combobox"
												className={cn(
													!field.value ? "text-muted-foreground" : "",
												)}
											>
												{field.value
													? availableLists.find(
															(list) => list.id === field.value,
														)?.name
													: "Select a list"}
												<ChevronDown className="ml-auto size-3" />
											</Button>
										</FormControl>
									</PopoverTrigger>
									<PopoverContent className="w-52 p-0">
										<Command>
											<CommandInput
												placeholder="Search for a list..."
												className="h-9"
											/>
											<CommandList>
												<CommandEmpty className="p-2 text-sm">
													No lists found
												</CommandEmpty>
												<CommandGroup>
													{availableLists.map((list) => (
														<CommandItem
															key={list.id}
															value={list.id}
															onSelect={() => {
																form.setValue("listId", list.id);
															}}
														>
															{list.name}
														</CommandItem>
													))}
												</CommandGroup>
											</CommandList>
										</Command>
									</PopoverContent>
								</Popover>
								<FormDescription>Pick a list for your task</FormDescription>
								<FormMessage />
							</FormItem>
						)}
					/>
					<FormField
						control={form.control}
						name="statusId"
						render={({ field }) => (
							<FormItem>
								<FormLabel>Status</FormLabel>
								<Select
									onValueChange={(value) => field.onChange(Number(value))}
									defaultValue={field.value.toString()}
								>
									<FormControl className="w-full">
										<SelectTrigger>
											<SelectValue placeholder="Select status" />
										</SelectTrigger>
									</FormControl>
									<SelectContent>
										<SelectItem value={Status.PENDING.id.toString()}>
											Pending
										</SelectItem>
										<SelectItem value={Status.IN_PROGRESS.id.toString()}>
											In Progress
										</SelectItem>
										<SelectItem value={Status.COMPLETED.id.toString()}>
											Completed
										</SelectItem>
									</SelectContent>
								</Select>
								<FormDescription>Set the status of the task</FormDescription>
								<FormMessage />
							</FormItem>
						)}
					/>
					<DialogFooter>
						<DialogClose asChild>
							<Button variant="outline" type="button">
								Cancel
							</Button>
						</DialogClose>
						<Button type="submit">Save Changes</Button>
					</DialogFooter>
				</form>
			</Form>
		</DialogContent>
	);
};

EditTaskForm.displayName = "EditTaskForm";

export default EditTaskForm;
