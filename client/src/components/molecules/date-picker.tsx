import { isBefore, sub } from "date-fns";
import type { FC } from "react";
import {
	Button,
	Calendar,
	Popover,
	PopoverContent,
	PopoverTrigger,
	type CalendarProps,
} from "@/components/atoms";
import { CalendarDays } from "lucide-react";
import { formatDate } from "@/lib/formatDate";

type Props = {
	date: Date | undefined;
	onChange: (date: Date | undefined) => void;
	calendarProps?: Omit<CalendarProps, "mode" | "selected" | "onSelect">;
};
const DatePicker: FC<Props> = (props) => {
	const { date, onChange, calendarProps } = props;

	return (
		<Popover>
			<PopoverTrigger asChild>
				<Button variant="outline">
					<CalendarDays className="size-4" />
					<span className="text-xs text-muted-foreground">
						{date ? formatDate(date) : "Select a date"}
					</span>
				</Button>
			</PopoverTrigger>
			<PopoverContent className="w-auto overflow-hidden p-0" align="start">
				<Calendar
					mode="single"
					selected={date}
					captionLayout="dropdown"
					onSelect={onChange}
					disabled={(date) => isBefore(date, sub(new Date(), { days: 1 }))}
					{...calendarProps}
				/>
			</PopoverContent>
		</Popover>
	);
};

DatePicker.displayName = "DatePicker";

export default DatePicker;
