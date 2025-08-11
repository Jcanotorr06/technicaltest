import type { FC } from "react";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import {
	Button,
	Checkbox,
	DialogClose,
	DialogContent,
	DialogFooter,
	DialogHeader,
	DialogTitle,
	Form,
	FormControl,
	FormDescription,
	FormField,
	FormItem,
	FormLabel,
	FormMessage,
	Input,
} from "@/components/atoms";
import { z } from "zod";
import { useCreateList } from "@/services";
import type { CreateListRequest } from "@/types";
import { useNavigate } from "@tanstack/react-router";
import { Loader } from "lucide-react";

const formSchema = z.object({
	title: z
		.string()
		.min(2, {
			error: "Title must be at least 2 characters long",
		})
		.max(100, {
			error: "Title cannot exceed 100 characters",
		}),
	isPublic: z.boolean(),
});

type FormSchemaType = z.infer<typeof formSchema>;

type Props = {
	onClose: () => void;
};

const AddListForm: FC<Props> = (props) => {
	const { onClose } = props;
	const createList = useCreateList();
	const navigate = useNavigate();
	const form = useForm<FormSchemaType>({
		resolver: zodResolver(formSchema),
		defaultValues: {
			title: "",
			isPublic: false,
		},
	});

	const onSubmit = async (data: FormSchemaType) => {
		const payload = {
			name: data.title,
			isPublic: data.isPublic,
		} satisfies CreateListRequest;
		const response = await createList.mutateAsync(payload);
		if (response.id) {
			navigate({ to: "/list/$listId", params: { listId: response.id } });
			onClose();
		}
	};
	return (
		<DialogContent className="sm:max-w-md">
			<DialogHeader>
				<DialogTitle>Add List</DialogTitle>
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
									<Input placeholder="List title" {...field} />
								</FormControl>
								<FormDescription>Enter a title for your list</FormDescription>
								<FormMessage />
							</FormItem>
						)}
					/>
					<FormField
						control={form.control}
						name="isPublic"
						render={({ field }) => (
							<FormItem className="flex flex-row items-center gap-2">
								<FormControl>
									<Checkbox
										checked={field.value}
										onCheckedChange={field.onChange}
									/>
								</FormControl>
								<FormLabel>Make List Public</FormLabel>
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
						<Button type="submit" disabled={createList.isPending}>
							Create
							{createList.isPending ? (
								<Loader className="ml-2 size-4 animate-spin" />
							) : null}
						</Button>
					</DialogFooter>
				</form>
			</Form>
		</DialogContent>
	);
};

AddListForm.displayName = "AddListForm";

export default AddListForm;
