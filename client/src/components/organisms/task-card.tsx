import { formatDate } from "@/lib/formatDate";
import { useCompleteTask, useDeleteTask } from "@/services";
import type { Task } from "@/types";
import {
  Calendar,
  Check,
  Ellipsis,
  Loader,
  Pencil,
  Trash2,
  User,
} from "lucide-react";
import type { FC } from "react";
import {
  AlertDialog,
  AlertDialogAction,
  AlertDialogCancel,
  AlertDialogContent,
  AlertDialogDescription,
  AlertDialogFooter,
  AlertDialogHeader,
  AlertDialogTitle,
  AlertDialogTrigger,
  Badge,
  Button,
  Separator,
  Skeleton,
} from "@/components/atoms";
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuTrigger,
} from "@/components/molecules";
import { Status } from "@/lib/const";

type Props = {
  task: Task;
  showButtons?: boolean;
};

const TaskCard: FC<Props> = (props) => {
  const { task, showButtons } = props;

  const taskStatus = Object.values(Status).find((status) => status.id === task.statusId) || Status.PENDING;
  const completeTask = useCompleteTask(task.id);
  const deleteTask = useDeleteTask(task.id);

  const handleComplete = async () => {
    await completeTask.mutateAsync();
  };

  const handleDelete = async () => {
    await deleteTask.mutateAsync();
  };

  if (deleteTask.isPending || completeTask.isPending) {
    return <Skeleton className="w-full h-20" />;
  }
  return (
    <div className="flex flex-row gap-1.5 mb-0.5 group/card relative">
      {task.statusId === Status.COMPLETED.id ? null : completeTask.isPending ? (
        <div className="size-fit mt-3 animate-spin">
          <Loader className="size-4 text-muted-foreground" />
        </div>
      ) : (
        <button
          className="cursor-pointer size-fit mt-3"
          type="button"
          onClick={handleComplete}
          disabled={completeTask.isPending}
        >
          <span className="size-4 border border-muted-foreground rounded-full flex items-center justify-center group/radio bg-background hover:bg-accent-foreground hover:text-accent transition duration-200">
            <Check className="size-4" />
          </span>
        </button>
      )}
      <div className="flex flex-col p-2">
        <div className="w-full text-sm font-semibold p-[1px] pl-0 wrap-break-word line-clamp-4 flex flex-row items-center gap-1">
          {task.title}
          <Badge variant={taskStatus.variant}>
            {taskStatus.label}
          </Badge>
        </div>
        <div className="text-xs text-muted-foreground w-full whitespace-break-spaces wrap-break-word line-clamp-4">
          {task.description}
        </div>
        <div className="text-xs text-muted-foreground flex flex-row items-center gap-1 mt-1 h-3">
          <Calendar className="size-3" />
          <span>{formatDate(task.dueDate)}</span>
          <Separator orientation="vertical" />
          <User className="size-3" />
          <span>{task.createdBy.split(";")[0]}</span>
        </div>
      </div>
      {showButtons ? (
        <AlertDialog>
          <DropdownMenu>
            <DropdownMenuTrigger asChild>
              <Button
                variant="ghost"
                size="sm"
                className="absolute right-2 top-2"
              >
                <Ellipsis className="size-4" />
              </Button>
            </DropdownMenuTrigger>
            <DropdownMenuContent>
              <DropdownMenuItem>
                <Pencil />
                <span>Edit</span>
              </DropdownMenuItem>

              <AlertDialogTrigger asChild>
                <DropdownMenuItem className="text-destructive">
                  <Trash2 className="text-destructive" />
                  <span>Delete</span>
                </DropdownMenuItem>
              </AlertDialogTrigger>
            </DropdownMenuContent>
          </DropdownMenu>
          <AlertDialogContent>
            <AlertDialogHeader>
              <AlertDialogTitle>
                Are you sure you want to delete this task?
              </AlertDialogTitle>
              <AlertDialogDescription>
                This action cannot be undone. This task will be permanently
                deleted from our system.
              </AlertDialogDescription>
            </AlertDialogHeader>
            <AlertDialogFooter>
              <AlertDialogCancel>Cancel</AlertDialogCancel>
              <AlertDialogAction onClick={handleDelete}>
                Delete
              </AlertDialogAction>
            </AlertDialogFooter>
          </AlertDialogContent>
        </AlertDialog>
      ) : null}
    </div>
  );
};

TaskCard.displayName = "TaskCard";

export default TaskCard;
