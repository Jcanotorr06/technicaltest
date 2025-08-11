import { RouterProvider } from "@tanstack/react-router";
import type { FC } from "react";
import { router } from "./main";
import { useToken } from "./context";

const App: FC = () => {
	const tokenContext = useToken();
	return <RouterProvider router={router} context={{ tokenContext }} />;
};

App.displayName = "App";

export default App;
