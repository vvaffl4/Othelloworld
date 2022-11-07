import { Box, Button, Card, CardActions, CardContent, CircularProgress, Container, Grid, Paper, Typography } from '@mui/material';
import { FC, useEffect, useState } from 'react';
import { fetchGame } from '../store/Game';
import { useAppDispatch, useAppSelector } from '../store/Hooks';
import Game from './Game';
import GameOption from './GameOption';


const Play: FC = () => {
	const dispatch = useAppDispatch();
	const [isLoading, setLoading] = useState(true);
	const hasGame = useAppSelector(state => state.game.hasGame);

	useEffect(() => {
		dispatch(fetchGame(
			_ => setLoading(false)
		));
	}, []);

	useEffect(() => {
		if (hasGame) {
			setLoading(false);
		}
	}, [hasGame]);

	return (
		<Box
			component="div"
			sx={{
				position: 'absolute',
				top: 0,
				bottom: 0,
				left: 0,
				right: 0
			}}
		>
		{
			isLoading
				? (
					<CircularProgress />
				)
				: hasGame
					? (
						<Game />
					)
					: (
						<GameOption />
					)
			}
		</Box>
	);
}

export default Play;