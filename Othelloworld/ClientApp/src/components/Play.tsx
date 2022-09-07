import { Box, Button } from '@mui/material';
import { FC } from 'react';
import { Outlet, useParams } from 'react-router-dom';
import { useAppDispatch } from '../store/Hooks';
import Game from './Game';


const Play: FC = () => {
	const dispatch = useAppDispatch();
	const gameToken = useParams()

	return (
		<>
			{gameToken
				? (
					<Game/>
				)
				: (
					<Box
						component="div"
					>
						<Button>
							Browse Games
						</Button>
						<Button>
							Create Game
						</Button>
					</Box>	
				)}
		</>
	);
}

export default Play;