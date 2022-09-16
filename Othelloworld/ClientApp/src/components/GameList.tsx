import { Box, Button, ButtonGroup, Container, Grid, LinearProgress, Paper, Table, TableBody, TableCell, TableContainer, TableHead, TableRow, Typography } from '@mui/material';
import { FC, Suspense, useEffect, useState } from 'react';
import GameItem from './GameItem';
import { useAppDispatch, useAppSelector } from '../store/Hooks';
import RefreshIcon from '@mui/icons-material/Refresh';
import AddIcon from '@mui/icons-material/Add';
import { fetchGames } from '../store/Games';
import { useNavigate } from 'react-router-dom';

const GameList: FC = () => {
	const dispatch = useAppDispatch();
	const navigate = useNavigate();

	const gameTokens = useAppSelector(state => state.games.allIds);
	const [loading, isLoading] = useState(true);

	console.log('browse game');

	useEffect(() => {
		dispatch(fetchGames());
		isLoading(true);
	}, []);

	const handlePlay = () => {
		navigate('../play')
	}

	const handleRefresh = () => {
		dispatch(fetchGames());
		isLoading(true);
	}

	return (
		<Box
			component="div"
			sx={{
				position: 'absolute',
				top: 100,
				left: 0,
				right: 0
			}}
		>
			<Container>
				<Typography
					variant="h1"
					color="white"
					sx={{
						padding: '100px 200px 0 0',
						fontWeight: 700,
						textAlign: 'right'
					}}
				>
					OTHELLO
				</Typography>
				<Typography
					variant="body2"
					color="white"
					sx={{
						padding: '100px 0 0 0',
						textAlign: 'right'
					}}
				>
					Reversi or Othello is a strategy board game for two players, played on an 8×8 uncheckered board. There are sixty-four identical game pieces called disks (often spelled "discs"), which are light on one side and dark on the other. Players take turns placing disks on the board with their assigned color facing up. During a play, any disks of the opponent's color that are in a straight line and bounded by the disk just placed and another disk of the current player's color are turned over to the current player's color. The object of the game is to have the majority of disks turned to display your color when the last playable empty square is filled.
				</Typography>
				<Paper sx={{
					mt: 10,
					overflow: 'hidden'
				}}>
					<div
						style={{ padding: '20px' }}
					>
						<div style={{ display: 'flex' }}>
							<div style={{ flexGrow: 1 }}>
								<Typography
									variant="h4"
								>
									Games
								</Typography>
							</div>
							<div style={{ flexGrow: 0 }}>
								<ButtonGroup>
									<Button
										variant="contained"
										endIcon={<RefreshIcon />}
										onClick={handleRefresh}
									>
										Refresh
									</Button>
									<Button
										variant="contained"
										endIcon={<AddIcon />}
										href="/play/new"
									>
										Create
									</Button>
								</ButtonGroup>
							</div>
						</div>
						<TableContainer>
							<Table aria-label="collapsible table">
								<TableHead>
									<TableRow>
										<TableCell />
										<TableCell>Name</TableCell>
										<TableCell align="right">Description</TableCell>
										<TableCell align="right">Host</TableCell>
										<TableCell align="right">Type</TableCell>
									</TableRow>
								</TableHead>
								<TableBody>
									<Suspense fallback={<LinearProgress />}>
										{gameTokens.map(gameToken => (
											<GameItem key={gameToken} token={gameToken} />
										))}
									</Suspense>
								</TableBody>
							</Table>
						</TableContainer>
					</div>
				</Paper>
				</Container>
			</Box>
	);
}

export default GameList;

