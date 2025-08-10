import { useToken } from "@/context";
import {
  InteractionRequiredAuthError,
  InteractionStatus,
  type SilentRequest,
} from "@azure/msal-browser";
import { useIsAuthenticated, useMsal } from "@azure/msal-react";
import { LoaderCircle } from "lucide-react";
import { useEffect, type FC, type PropsWithChildren } from "react";

/**
 * Authorization component for handling user authentication.
 * @param {PropsWithChildren} param0
 * @returns {JSX.Element}
 */
const Authorization: FC<PropsWithChildren> = ({ children }) => {
  const { accounts, instance, inProgress } = useMsal();
  const isAuthenticated = useIsAuthenticated();
  const { token, setToken } = useToken();

  // biome-ignore lint/correctness/useExhaustiveDependencies: e
  useEffect(() => {
    const accessTokenRequest = {
      scopes: [
        "openid",
        "profile",
        "offline_access",
        "email",
        "api://5c6bb0b4-f5cf-4193-954d-a2c63ad78bef/user_impersonation",
      ],
      account: accounts[0],
      forceRefresh: true,
      refreshTokenExpirationOffsetSeconds: 7200,
    } satisfies SilentRequest;

    const logoutRequest = {
      account: accounts[0],
      postLogoutRedirectUri: "/",
    };

    if (
      inProgress === InteractionStatus.None &&
      accounts.length > 0 &&
      !token
    ) {
      instance
        .acquireTokenSilent(accessTokenRequest)
        .then((response) => {
          console.log("Setting token", response.accessToken);
          setToken(response.accessToken);
        })
        .catch((e) => {
          if (e instanceof InteractionRequiredAuthError) {
            instance.acquireTokenRedirect(accessTokenRequest);
          }
          instance.logoutRedirect(logoutRequest);
        });
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [inProgress, token]);

  // biome-ignore lint/correctness/useExhaustiveDependencies: e
  useEffect(() => {
    if (
      !isAuthenticated &&
      inProgress === InteractionStatus.None &&
      accounts.length === 0
    ) {
      instance
        .loginRedirect({
          scopes: [
            "openid",
            "profile",
            "offline_access",
            "email",
            "api://5c6bb0b4-f5cf-4193-954d-a2c63ad78bef/user_impersonation",
          ],
        })
        .then(() => {
          console.log("Login successful");
        })
        .catch((error) => {
          console.error("Login failed", error);
        });
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [isAuthenticated, inProgress]);

  if (!token) {
    return (
      <div className="w-screen h-screen flex items-center justify-center bg-background">
        <div className="flex flex-row items-center justify-center gap-4 w-fit flex-wrap">
          <LoaderCircle className="w-12 h-12 animate-spin" />
          <h3 className="text-2xl font-semibold">Authenticating...</h3>
        </div>
      </div>
    );
  }
  return <>{children}</>;
};

Authorization.displayName = "Authorization";
export default Authorization;
