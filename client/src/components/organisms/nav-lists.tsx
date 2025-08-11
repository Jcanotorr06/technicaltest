import {
	Button,
	Collapsible,
	CollapsibleContent,
	CollapsibleTrigger,
	Dialog,
	DialogTrigger,
} from "@/components/atoms";
import {
	SidebarGroup,
	SidebarGroupContent,
	SidebarGroupLabel,
	SidebarMenu,
	SidebarMenuButton,
	SidebarMenuItem,
} from "@/components/molecules";
import { AddListForm } from "@/components/organisms";
import type { List } from "@/types";
import { Link } from "@tanstack/react-router";
import { ChevronRight, CirclePlus, Hash } from "lucide-react";
import { type FC, useState } from "react";

type Props = {
	label: string;
	lists: List[];
	isLoading: boolean;
	isError: boolean;
};
const NavLists: FC<Props> = (props) => {
	const { lists, label } = props;
	const [openForm, setOpenForm] = useState(false);
	return (
		<Collapsible title="Lists" defaultOpen={true} className="group/collapsible">
			<Dialog open={openForm} onOpenChange={setOpenForm}>
				<SidebarMenu>
					<SidebarMenuItem>
						<SidebarGroup>
							<SidebarGroupLabel className="group/label hover:bg-sidebar-accent flex flex-row items-center gap-2">
								<span className="font-medium">{label}</span>
								<DialogTrigger asChild>
									<Button
										size="icon"
										variant="ghost"
										className="ml-auto opacity-0 transition-opacity duration-200 ease-in-out group-hover/label:opacity-60"
									>
										<CirclePlus className="size-4" />
									</Button>
								</DialogTrigger>
								<AddListForm onClose={() => setOpenForm(false)} />
								<CollapsibleTrigger>
									<ChevronRight className="ml-auto transition-[transform,opacity,rotate] group-data-[state=open]/collapsible:rotate-90 opacity-0 group-hover/sidebar:opacity-60 " />
								</CollapsibleTrigger>
							</SidebarGroupLabel>
							<CollapsibleContent>
								<SidebarGroupContent>
									{lists.map((list) => (
										<SidebarMenuButton key={list.id} asChild>
											<Link
												to="/list/$listId"
												params={{ listId: list.id }}
												activeProps={{
													className: "bg-sidebar-accent",
												}}
											>
												<Hash className="size-4 opacity-60 text-sidebar-foreground" />
												<span className="font-medium">{list.name}</span>
												<span className="ml-auto text-xs text-muted-foreground">
													{list.taskCount}
												</span>
											</Link>
										</SidebarMenuButton>
									))}
								</SidebarGroupContent>
							</CollapsibleContent>
						</SidebarGroup>
					</SidebarMenuItem>
				</SidebarMenu>
			</Dialog>
		</Collapsible>
	);
};

NavLists.displayName = "NavLists";

export default NavLists;
