import { createFileRoute, Link } from "@tanstack/react-router";

export const Route = createFileRoute("/")({
	component: Index,
});

function Index() {
	return (
		<main className="min-h-screen flex flex-col items-center justify-center bg-gradient-to-br from-blue-50 via-white to-purple-100 px-4 py-12">
			<section className="w-full max-w-3xl text-center">
				<img
					src="/logo.png"
					alt="Logo"
					className="mx-auto mb-6 w-24 h-24 drop-shadow-lg"
				/>
				<h1 className="text-4xl md:text-6xl font-extrabold bg-gradient-to-r from-blue-600 via-purple-500 to-pink-500 bg-clip-text text-transparent mb-4">
					Organize Your Day, Achieve More
				</h1>
				<p className="text-lg md:text-2xl text-gray-700 mb-8">
					Welcome to your all-in-one task manager. Plan, track, and accomplish
					your goals with ease.
				</p>
				<Link
					to="/today"
					className="inline-block px-8 py-3 rounded-full bg-gradient-to-r from-blue-500 to-purple-500 text-white font-semibold shadow-lg hover:scale-105 hover:from-blue-600 hover:to-pink-500 transition-transform duration-200"
				>
					Get Started
				</Link>
			</section>
			<section className="mt-16 grid grid-cols-1 md:grid-cols-3 gap-8 w-full max-w-4xl">
				<div className="bg-white rounded-xl shadow-md p-6 flex flex-col items-center hover:shadow-xl transition-shadow">
					<svg
						className="w-10 h-10 text-blue-500 mb-3"
						fill="none"
						stroke="currentColor"
						strokeWidth="2"
						viewBox="0 0 24 24"
					>
						<title>Checkmark in Circle</title>
						<path
							strokeLinecap="round"
							strokeLinejoin="round"
							d="M9 12l2 2l4-4m5 2a9 9 0 11-18 0a9 9 0 0118 0z"
						/>
					</svg>
					<h2 className="font-bold text-xl mb-2">Easy Task Management</h2>
					<p className="text-gray-600">
						Create, edit, and organize your tasks and lists effortlessly.
					</p>
				</div>
				<div className="bg-white rounded-xl shadow-md p-6 flex flex-col items-center hover:shadow-xl transition-shadow">
					<svg
						className="w-10 h-10 text-purple-500 mb-3"
						fill="none"
						stroke="currentColor"
						strokeWidth="2"
						viewBox="0 0 24 24"
					>
						<title>Clock in Circle</title>
						<path
							strokeLinecap="round"
							strokeLinejoin="round"
							d="M12 8v4l3 3m6-3a9 9 0 11-18 0a9 9 0 0118 0z"
						/>
					</svg>
					<h2 className="font-bold text-xl mb-2">Stay On Track</h2>
					<p className="text-gray-600">
						View your tasks for today, upcoming, and completed in one place.
					</p>
				</div>
				<div className="bg-white rounded-xl shadow-md p-6 flex flex-col items-center hover:shadow-xl transition-shadow">
					<svg
						className="w-10 h-10 text-pink-500 mb-3"
						fill="none"
						stroke="currentColor"
						strokeWidth="2"
						viewBox="0 0 24 24"
					>
						<title>Lock in Circle</title>
						<path
							strokeLinecap="round"
							strokeLinejoin="round"
							d="M17 9V7a5 5 0 00-10 0v2a2 2 0 00-2 2v5a2 2 0 002 2h10a2 2 0 002-2v-5a2 2 0 00-2-2z"
						/>
					</svg>
					<h2 className="font-bold text-xl mb-2">Secure & Private</h2>
					<p className="text-gray-600">
						Your data is protected and only accessible by you.
					</p>
				</div>
			</section>
		</main>
	);
}
