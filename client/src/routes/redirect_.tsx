import { useMsal } from "@azure/msal-react";
import { createFileRoute, Navigate } from "@tanstack/react-router";
import { LoaderCircle } from "lucide-react";

export const Route = createFileRoute("/redirect_")({
  component: Redirect,
});

function Redirect() {
  const { accounts } = useMsal();

  // Redirect to home if user is authenticated
  if (accounts.length > 0) {
    return <Navigate to="/" replace />;
  }

  return (
    <div className="w-screen h-screen flex items-center justify-center bg-background">
      <div className="flex flex-row items-center justify-center gap-4 w-fit flex-wrap">
        <LoaderCircle className="w-12 h-12 animate-spin" />
        <h3 className="text-2xl font-semibold">Redirecting...</h3>
      </div>
    </div>
  );
}
