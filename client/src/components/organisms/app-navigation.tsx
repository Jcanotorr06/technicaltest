import { Separator } from "@/components/atoms";
import {
  Breadcrumb,
  BreadcrumbItem,
  BreadcrumbLink,
  BreadcrumbList,
  BreadcrumbPage,
  BreadcrumbSeparator,
  SidebarTrigger,
} from "@/components/molecules";
import { isMatch, Link, useMatches } from "@tanstack/react-router";

const AppNavigation = () => {
  const matches = useMatches();

  const matchesWithCrumbs = matches.filter((match) =>
    isMatch(match, "loaderData.crumb")
  );

  const breadcrumbItems = matchesWithCrumbs
    .filter((match) => match.loaderData?.crumb)
    .map(({ pathname, loaderData }) => ({
      href: pathname,
      label: loaderData?.crumb,
    }));
  return (
    <header className="flex h-16 shrink-0 items-center gap-2 transition-[width,height] ease-linear group-has-data-[collapsible=icon]/sidebar-wrapper:h-12">
      <div className="flex items-center gap-2 px-4">
        <SidebarTrigger className="-ml-1" />
        <Separator
          orientation="vertical"
          className="mr-2 data-[orientation=vertical]:h-4"
        />
        <Breadcrumb>
          <BreadcrumbList>
            {breadcrumbItems.map((item, index) => (
              <>
                <BreadcrumbItem key={`${item.href}-${index}`}>
                  {index === breadcrumbItems.length - 1 ? (
                    <BreadcrumbPage>{item.label}</BreadcrumbPage>
                  ) : (
                    <BreadcrumbLink asChild>
                      <Link to={item.href}>{item.label}</Link>
                    </BreadcrumbLink>
                  )}
                </BreadcrumbItem>
                {index < breadcrumbItems.length - 1 && (
                  <BreadcrumbSeparator className="hidden md:block" />
                )}
              </>
            ))}
          </BreadcrumbList>
        </Breadcrumb>
      </div>
    </header>
  );
};

AppNavigation.displayName = "AppNavigation";

export default AppNavigation;
