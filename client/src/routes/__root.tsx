import { SidebarInset, SidebarProvider } from "@/components/molecules";
import { AppNavigation, AppSidebar } from "@/components/organisms";
import type { useToken } from "@/context";
import { getListsQueryOptions, getPublicListsQueryOptions } from "@/services";
import type { QueryClient } from "@tanstack/react-query";
import { createRootRouteWithContext, Outlet } from "@tanstack/react-router";
import { TanStackRouterDevtools } from "@tanstack/react-router-devtools";
import { LoaderCircle } from "lucide-react";

type RootRouteContext = {
	queryClient: QueryClient;
	tokenContext: ReturnType<typeof useToken>;
};

export const Route = createRootRouteWithContext<RootRouteContext>()({
	component: RootLayout,
	pendingComponent: () => (
		<div className="w-screen h-screen flex items-center justify-center bg-background">
			<div className="flex flex-row items-center justify-center gap-4 w-fit flex-wrap">
				<LoaderCircle className="w-12 h-12 animate-spin" />
				<h3 className="text-2xl font-semibold">Loading...</h3>
			</div>
		</div>
	),
	loader: ({ context }) => {
		const { queryClient, tokenContext } = context;
		if (tokenContext.headers.has("Authorization")) {
			queryClient.ensureQueryData(getListsQueryOptions(tokenContext.headers));
			queryClient.ensureQueryData(
				getPublicListsQueryOptions(tokenContext.headers),
			);
		}
	},
});

function RootLayout() {
	return (
		<SidebarProvider>
			<AppSidebar />
			<SidebarInset>
				<AppNavigation />
				<div className="flex flex-1 flex-col gap-4 pt-0">
					<Outlet />
				</div>
			</SidebarInset>
			{import.meta.env.DEV ? <TanStackRouterDevtools /> : null}
		</SidebarProvider>
	);
}
