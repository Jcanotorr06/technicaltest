import { zodResolver } from "@hookform/resolvers/zod";
import { ChevronDown, PlusCircle } from "lucide-react";
import { type FC, useState } from "react";
import { useForm } from "react-hook-form";
import { z } from "zod";
import {
  AutoSizedTextArea,
  Button,
  Card,
  CardContent,
  CardFooter,
  Form,
  FormControl,
  FormField,
  FormItem,
  Input,
  Popover,
  PopoverContent,
  PopoverTrigger,
} from "@/components/atoms";
import {
  Command,
  CommandInput,
  CommandItem,
  DatePicker,
} from "@/components/molecules";
import { useSuspenseQuery } from "@tanstack/react-query";
import {
  getListsQueryOptions,
  getPublicListsQueryOptions,
  useCreateTask,
} from "@/services";
import { useToken } from "@/context";
import { cn } from "@/lib/utils";
import { CommandEmpty, CommandGroup, CommandList } from "cmdk";
import { useParams } from "@tanstack/react-router";
import type { CreateTaskRequest } from "@/types";

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
});

type FormSchemaType = z.infer<typeof formSchema>;

type Props = {
  defaultOpen?: boolean;
  onCancel?: () => void;
  initialValues?: FormSchemaType;
};

const AddTaskForm: FC<Props> = (props) => {
  const { defaultOpen, onCancel, initialValues } = props;
  const { headers } = useToken();
  const { listId } = useParams({ strict: false });
  const [open, setOpen] = useState(false);

  const form = useForm<FormSchemaType>({
    resolver: zodResolver(formSchema),
    defaultValues: {
      title: initialValues?.title || "",
      description: initialValues?.description || "",
      dueDate: initialValues?.dueDate || new Date(),
      listId: initialValues?.listId || listId || "",
    },
  });

  const createTask = useCreateTask();

  const myListsQuery = useSuspenseQuery(getListsQueryOptions(headers));
  const publicListsQuery = useSuspenseQuery(
    getPublicListsQueryOptions(headers)
  );

  const myLists = myListsQuery.data;
  const publicLists = publicListsQuery.data;

  const availableLists = myLists.concat(
    publicLists.filter(
      (list) => !myLists.some((myList) => myList.id === list.id)
    )
  );

  const handleToggle = () => {
    setOpen((o) => !o);
    if (onCancel) {
      onCancel();
    }
  };

  const onSubmit = async (data: FormSchemaType) => {
    const payload = {
      title: data.title,
      description: data.description || "",
      dueDate: data.dueDate,
      listId: data.listId,
      assignedTo: "",
      status: 1,
    } satisfies CreateTaskRequest;

    const result = await createTask.mutateAsync(payload);
    if (result.id) {
      form.reset();
      setOpen(false);
      if (onCancel) {
        onCancel();
      }
    }
  };
  return (
    <>
      {open || defaultOpen ? (
        <Form {...form}>
          <form onSubmit={form.handleSubmit(onSubmit)}>
            <Card className="shadow-none py-2">
              <CardContent className="flex flex-col gap-0.5">
                <FormField
                  control={form.control}
                  name="title"
                  render={({ field }) => (
                    <FormItem className="relative">
                      <FormControl>
                        <Input
                          //className="absolute h-0 w-0 opacity-0"
                          className="border-none! outline-none! shadow-none! text-sm text-accent-foreground p-0 font-semibold h-[unset] placeholder:opacity-20"
                          placeholder="Enter a title for your task"
                          {...field}
                        />
                      </FormControl>
                    </FormItem>
                  )}
                />
                <FormField
                  control={form.control}
                  name="description"
                  render={({ field }) => (
                    <FormItem>
                      <FormControl>
                        <AutoSizedTextArea
                          placeholder="Task description"
                          minRows={1}
                          className="resize-none border-none outline-none shadow-none text-xs text-muted-foreground p-0 font-normal h-[unset] w-full"
                          {...field}
                        />
                      </FormControl>
                    </FormItem>
                  )}
                />
                <div className="flex flex-row items-center gap-2 mt-2">
                  <FormField
                    control={form.control}
                    name="dueDate"
                    render={({ field }) => (
                      <FormItem>
                        <FormControl>
                          <DatePicker
                            date={field.value}
                            onChange={field.onChange}
                          />
                        </FormControl>
                      </FormItem>
                    )}
                  />
                </div>
              </CardContent>
              <CardFooter className="flex gap-2 justify-start flex-wrap border-t-1 pt-2">
                <FormField
                  control={form.control}
                  name="listId"
                  render={({ field }) => (
                    <FormItem>
                      <Popover>
                        <PopoverTrigger asChild>
                          <FormControl>
                            {/** biome-ignore lint/a11y/useSemanticElements:  Button */}
                            <Button
                              variant="outline"
                              role="combobox"
                              className={cn(
                                !field.value ? "text-muted-foreground" : ""
                              )}
                            >
                              {field.value
                                ? availableLists.find(
                                    (list) => list.id === field.value
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
                    </FormItem>
                  )}
                />
                <Button
                  type="button"
                  variant="outline"
                  onClick={handleToggle}
                  className="ml-auto"
                >
                  Cancel
                </Button>
                <Button type="submit">Add Task</Button>
              </CardFooter>
            </Card>
          </form>
        </Form>
      ) : (
        <Button
          variant="ghost"
          size="lg"
          className="w-full text-md text-left justify-start"
          onClick={handleToggle}
        >
          <PlusCircle className="size-6" />
          Add Task
        </Button>
      )}
    </>
  );
};

AddTaskForm.displayName = "AddTaskForm";

export default AddTaskForm;
