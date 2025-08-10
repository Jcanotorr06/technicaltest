import { useQuery } from "@tanstack/react-query";
import { useToken } from "@/context";
import type { List } from "@/types";

/**
 * Custom hook to fetch lists.
 * @returns {ReturnType<typeof useQuery>} - The resulting query object
 *
 * See https://tanstack.com/query/latest/docs/framework/react/reference/useQuery for API details.
 */
export const useGetLists = () => {
  const { headers } = useToken();
  return useQuery({
    queryKey: ["lists"],
    queryFn: async () => {
      const url = `${import.meta.env.VITE_API_URL}/lists`;
      const request = new Request(url, {
        method: "GET",
        headers: headers,
      });

      const response = await fetch(request);

      if (!response.ok) {
        throw new Error("Failed to fetch lists");
      }
      return response.json() as Promise<List[]>;
    },
  });
};
