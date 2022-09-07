import { FC } from 'react';
import { Outlet } from 'react-router-dom';
import { useAppDispatch } from '../store/Hooks';

const Play: FC = () => {
	const dispatch = useAppDispatch();


	return (
		<>
			<Outlet />
		</>
	);
}

export default Play;