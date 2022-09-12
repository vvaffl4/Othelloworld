import { Box, Button, Card, CardActions, CardContent, Container, Grid, Paper, Typography } from '@mui/material';
import { FC, useEffect } from 'react';
import { Outlet, useParams } from 'react-router-dom';
import { fetchGame } from '../store/Game';
import { useAppDispatch, useAppSelector } from '../store/Hooks';
import Game from './Game';


const Play: FC = () => {
	const dispatch = useAppDispatch();
	const hasGame = useAppSelector(state => state.game.hasGame);

	useEffect(() => {
		dispatch(fetchGame());
	}, []);

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
			{ hasGame
				? (
					<Game/>
				)
				: (
					<Box
						component="div"
						sx={{
							marginTop: '500px'
						}}
					>
						<Container>
							<Paper
								elevation={4}
								sx={{ p: 4 }}
							>
								<Grid
									container
									spacing={4}
								>
									<Grid item xs={6}>
										<Card variant="outlined">
											<CardContent>
												<Typography sx={{ fontSize: 14 }} color="text.secondary" gutterBottom>
													Normal
												</Typography>
												<Typography variant="h5" component="div">
													Browse for a game
												</Typography>
												<Typography sx={{ mb: 1.5 }} color="text.secondary">
													Are you up for the challenge?
												</Typography>
												<Typography variant="body2">
													Look for games hosted by people all over the world! Challenge another player!
												</Typography>
											</CardContent>
											<CardActions>
												<Button
													fullWidth
													variant="contained"
													href="browse"
												>
													Browse Games
												</Button>
											</CardActions>
										</Card>
									</Grid>
									<Grid item xs={6}>
										<Card variant="outlined" elevation={12}>
											<CardContent>
												<Typography sx={{ fontSize: 14 }} color="text.secondary" gutterBottom>
													Normal
												</Typography>
												<Typography variant="h5" component="div">
													Create a new game.
												</Typography>
												<Typography sx={{ mb: 1.5 }} color="text.secondary">
													Who will you face?!
												</Typography>
												<Typography variant="body2">
													Create a new game and play with people from all over the world!
												</Typography>
											</CardContent>
											<CardActions>
												<Button
													fullWidth
													variant="contained"
													href="new"
												>
													Create Game
												</Button>
											</CardActions>
										</Card>
									</Grid>
								</Grid>
							</Paper>
						</Container>
					</Box>	
				)}
		</Box>
	);
}

export default Play;