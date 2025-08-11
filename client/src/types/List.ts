export type List = {
  id: string;
  name: string;
  taskCount: number;
};

export type CreateListRequest = {
  name: string;
  isPublic: boolean;
};
