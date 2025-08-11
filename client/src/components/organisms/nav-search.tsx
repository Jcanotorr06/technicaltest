import { Button } from "@/components/atoms";
import {
	CommandDialog,
	CommandEmpty,
	CommandGroup,
	CommandInput,
	CommandItem,
	CommandList,
	CommandSeparator,
} from "@/components/molecules";
import { useUserTasks } from "@/services";
import { Link } from "@tanstack/react-router";
import { Command } from "cmdk";
import {
	Calendar1,
	CalendarDays,
	CircleCheck,
	Home,
	Search,
} from "lucide-react";
import { type FC, useEffect, useState } from "react";
import { useDebounce } from "use-debounce";

const NavSearch: FC = () => {
	const [open, setOpen] = useState(false);
	const [value, setValue] = useState("");
	const [debouncedValue] = useDebounce(value, 500);

	const userTaks = useUserTasks(debouncedValue);

	useEffect(() => {
		const down = (e: KeyboardEvent) => {
			if (e.key === "j" && (e.metaKey || e.ctrlKey)) {
				e.preventDefault();
				setOpen((open) => !open);
			}
		};

		document.addEventListener("keydown", down);
		return () => document.removeEventListener("keydown", down);
	}, []);

	const handleCommandItemSelect = () => {
		setOpen(false);
	};

	const handleSearch = (input: string) => {
		setValue(input);
	};

	return (
		<>
			<Button
				variant="outline"
				size="lg"
				className="justify-start"
				onClick={() => setOpen(true)}
			>
				<Search className="size-4" />
				Search
			</Button>
			<CommandDialog open={open} onOpenChange={setOpen}>
				<CommandInput
					value={value}
					onValueChange={handleSearch}
					placeholder="Search..."
				/>
				<CommandList>
					<CommandEmpty>No results found</CommandEmpty>
					<CommandGroup heading="Navigation">
						<CommandItem asChild>
							<Link to="/about" className="bg-background hidden">
								<Home />
								About
							</Link>
						</CommandItem>
						<CommandItem asChild onSelect={handleCommandItemSelect}>
							<Link
								to="/"
								className="bg-background"
								activeProps={{ className: "bg-sidebar-accent" }}
							>
								<Home />
								Home
							</Link>
						</CommandItem>
						<CommandItem asChild onSelect={handleCommandItemSelect}>
							<Link
								to="/today"
								activeProps={{ className: "bg-sidebar-accent" }}
							>
								<Calendar1 />
								Today
							</Link>
						</CommandItem>
						<CommandItem asChild onSelect={handleCommandItemSelect}>
							<Link
								to="/upcoming"
								activeProps={{ className: "bg-sidebar-accent" }}
							>
								<CalendarDays />
								Upcoming
							</Link>
						</CommandItem>
						<CommandItem asChild onSelect={handleCommandItemSelect}>
							<Link
								to="/completed"
								activeProps={{ className: "bg-sidebar-accent" }}
							>
								<CircleCheck />
								Completed
							</Link>
						</CommandItem>
					</CommandGroup>
					<CommandSeparator />
					<CommandGroup heading="Tasks">
						{userTaks.isPending && debouncedValue ? (
							<Command.Loading>Loading</Command.Loading>
						) : (
							userTaks.data?.map((task) => (
								<CommandItem
									key={task.id}
									asChild
									onSelect={handleCommandItemSelect}
									keywords={[task.title, task.description, task.listName || ""]}
									value={task.title}
								>
									<Link
										to="/list/$listId"
										params={{
											listId: task.listId,
										}}
										hash={task.id}
										className="bg-background"
										activeProps={{ className: "bg-sidebar-accent" }}
									>
										{task.title}
									</Link>
								</CommandItem>
							))
						)}
					</CommandGroup>
				</CommandList>
			</CommandDialog>
		</>
	);
};

export default NavSearch;
