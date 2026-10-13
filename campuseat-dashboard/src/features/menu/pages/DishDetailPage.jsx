import { useParams, Link } from "react-router-dom";

function DishDetailPage() {
	const { id } = useParams(); // from /dish/:id

	return (
		<div>
			<p>Showing details for dish #{id}</p>
			<Link to="/">← Back to menu</Link>
		</div>
	);
}

export default DishDetailPage;