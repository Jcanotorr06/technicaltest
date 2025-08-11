import { add, format, isBefore, isToday, isTomorrow } from "date-fns";

export const formatDate = (date: Date) => {
  if (isToday(date)) {
    return "Today";
  }
  if (isTomorrow(date)) {
    return "Tomorrow";
  }
  if (isBefore(date, add(new Date(), { days: 7 }))) {
    return format(date, "EEEE");
  }
  return format(date, "MMM dd");
};
