import { createFileRoute, Link } from "@tanstack/react-router";

export const Route = createFileRoute("/")({
	component: Index,
	loader: () => {
		return {
			crumb: "Home",
		};
	},
});

function Index() {
	return (
		<div className="p-2">
			<h3>Welcome Home!</h3>
			<Link
				to="/list/$listId"
				params={{
					listId: "123",
				}}
			>
				Go to List
			</Link>
		</div>
	);
}
