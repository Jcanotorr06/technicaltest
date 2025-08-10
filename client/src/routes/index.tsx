import { useMsal } from "@azure/msal-react";
import { Button } from "@/components/atoms";
import { createFileRoute } from "@tanstack/react-router";
import { useGetLists } from "@/services";

export const Route = createFileRoute("/")({
  component: Index,
});

function Index() {
  const { instance } = useMsal();
  const listsQuery = useGetLists();

  console.log("Lists Query:", listsQuery);
  return (
    <div className="p-2">
      <h3>Welcome Home!</h3>
      <Button onClick={() => instance.logout()}>Sign out</Button>
    </div>
  );
}
