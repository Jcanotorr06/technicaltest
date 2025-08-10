import {
  createContext,
  useContext,
  useEffect,
  useMemo,
  useState,
  type FC,
  type PropsWithChildren,
} from "react";

type TokenContextType = {
  token: string;
  headers: Headers;
  setToken: (token: string) => void;
};

const TokenContext = createContext<TokenContextType | null>(null);

/**
 * Context provider for the token and headers.
 *
 * @param {PropsWithChildren} param0
 * @returns {JSX.Element}
 */
const TokenProvider: FC<PropsWithChildren> = ({ children }) => {
  const [token, setToken] = useState<string>("");
  const [headers, setHeaders] = useState<Headers>(new Headers());

  const value = useMemo(() => {
    const setTokenValue = (newToken: string) => {
      setToken(newToken);
    };
    return { token, setToken: setTokenValue };
  }, [token]);

  useEffect(() => {
    if (token) {
      setHeaders(() => {
        const newHeaders = new Headers();
        newHeaders.append(
          "x-functions-key",
          import.meta.env.VITE_FUNCTIONS_KEY
        );
        newHeaders.append(
          "Ocp-Apim-Subscription-Key",
          import.meta.env.VITE_APIM_KEY
        );
        newHeaders.append("Content-Type", "application/json");
        newHeaders.append("Authorization", `Bearer ${token}`);
        return newHeaders;
      });
    }
  }, [token]);

  return (
    <TokenContext.Provider value={{ ...value, headers }}>
      {children}
    </TokenContext.Provider>
  );
};

/**
 * Custom hook to access the token context.
 * @returns {TokenContextType} The token context value.
 */
const useToken = () => {
  const context = useContext(TokenContext);
  if (!context) {
    throw new Error("useToken must be used within a TokenProvider");
  }
  return context;
};

// eslint-disable-next-line react-refresh/only-export-components
export { TokenContext, TokenProvider, useToken };
