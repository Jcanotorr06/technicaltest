import {
	Button,
	Dialog,
	DialogContent,
	DialogHeader,
	DialogTrigger,
} from "@/components/atoms";
import {
	NavUser,
	Sidebar,
	SidebarContent,
	SidebarGroup,
	SidebarGroupContent,
	SidebarHeader,
	SidebarMenu,
	SidebarMenuButton,
	SidebarMenuItem,
} from "@/components/molecules";
import { AddTaskForm, NavSearch } from "@/components/organisms";
import { useToken } from "@/context";
import { getListsQueryOptions, getPublicListsQueryOptions } from "@/services";
import { DialogTitle } from "@radix-ui/react-dialog";
import { useSuspenseQuery } from "@tanstack/react-query";
import { Link } from "@tanstack/react-router";
import {
	Calendar1,
	CalendarDays,
	CircleCheck,
	CirclePlus,
	Home,
} from "lucide-react";
import { type FC, useState } from "react";
import NavLists from "./nav-lists";

const AppSidebar: FC = () => {
	const { headers } = useToken();
	const [open, setOpen] = useState(false);
	const myListsQuery = useSuspenseQuery(getListsQueryOptions(headers));
	const publicListsQuery = useSuspenseQuery(
		getPublicListsQueryOptions(headers),
	);

	return (
		<Sidebar variant="inset" className="group/sidebar">
			<SidebarHeader>
				<NavUser />
			</SidebarHeader>
			<SidebarContent>
				<SidebarGroup>
					<SidebarGroupContent className="flex flex-col gap-2">
						<SidebarMenu>
							<SidebarMenuItem className="flex flex-col gap-2">
								<SidebarMenuButton tooltip="Add Task" asChild className="p-0">
									<div className="w-full p-0">
										<Dialog open={open} onOpenChange={setOpen}>
											<DialogTrigger asChild>
												<Button
													variant="default"
													size="lg"
													className="justify-start w-full"
												>
													<CirclePlus className="size-4" />
													Add Task
												</Button>
											</DialogTrigger>
											<DialogContent>
												<DialogHeader>
													<DialogTitle>Add Task</DialogTitle>
												</DialogHeader>
												<AddTaskForm
													defaultOpen
													onCancel={() => setOpen(false)}
												/>
											</DialogContent>
										</Dialog>
									</div>
								</SidebarMenuButton>
								<SidebarMenuButton asChild>
									<NavSearch />
								</SidebarMenuButton>
							</SidebarMenuItem>
							<SidebarMenuItem></SidebarMenuItem>
							<SidebarMenuItem>
								<SidebarMenuButton asChild>
									<Link to="/" activeProps={{ className: "bg-sidebar-accent" }}>
										<Home className="size-4" />
										Home
									</Link>
								</SidebarMenuButton>
								<SidebarMenuButton asChild>
									<Link
										to="/today"
										activeProps={{ className: "bg-sidebar-accent" }}
									>
										<Calendar1 className="size-4" />
										Today
									</Link>
								</SidebarMenuButton>

								<SidebarMenuButton asChild>
									<Link
										to="/upcoming"
										activeProps={{ className: "bg-sidebar-accent" }}
									>
										<CalendarDays className="size-4" />
										Upcoming
									</Link>
								</SidebarMenuButton>
								<SidebarMenuButton asChild>
									<Link
										to="/completed"
										activeProps={{ className: "bg-sidebar-accent" }}
									>
										<CircleCheck className="size-4" />
										Completed
									</Link>
								</SidebarMenuButton>
							</SidebarMenuItem>
						</SidebarMenu>
					</SidebarGroupContent>
				</SidebarGroup>

				<NavLists
					lists={publicListsQuery.data}
					label="Public Lists"
					isLoading={publicListsQuery.isPending}
					isError={publicListsQuery.isError}
				/>
				<NavLists
					lists={myListsQuery.data}
					label="My Lists"
					isLoading={myListsQuery.isPending}
					isError={myListsQuery.isError}
				/>
			</SidebarContent>
		</Sidebar>
	);
};

AppSidebar.displayName = "AppSidebar";
export default AppSidebar;
