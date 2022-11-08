import { Box, Button, Card, CardContent, CircularProgress, Dialog, DialogActions, DialogContent, DialogContentText, DialogTitle, Divider, Paper, Stack, Typography } from '@mui/material';
import { FC, useEffect, useState } from 'react';
import { confirmResults } from '../api';
import Game, { GameStatus } from '../model/Game';
import { Color, GameResult } from '../model/PlayerInGame';
import { countColors, createTurnBoards } from '../services/BoardService';
import { dropGame, setGame } from '../store/Game';
import { useAppDispatch, useAppSelector } from '../store/Hooks';
import CircleIcon from '@mui/icons-material/Circle';
import CircleOutlinedIcon from '@mui/icons-material/CircleOutlined';
import { AreaChart, Area, XAxis, YAxis, CartesianGrid, Tooltip, ResponsiveContainer, ReferenceLine } from 'recharts';




interface GameResultProps {
	finished: boolean
}

const GameResults: FC<GameResultProps> = ({ finished }) => {
	const dispatch = useAppDispatch();
	const auth = useAppSelector(state => state.auth);

	const [game, updateGame] = useState<Game>(null!)

	useEffect(() => {
		if (finished) {
			confirmResults(auth)
				.then((game) => {
					dispatch(setGame(game));
					updateGame(game);
				});
		}
	}, [finished]);

	const handleExit = () => {
		dispatch(dropGame());
	}

	if (!finished) return null;
	if (game === null) return (
		<Box
			component="div"
			sx={{
				position: 'absolute',
				top: 0,
				bottom: 0,
				left: 0,
				right: 0,
				p: 10,
				backgroundColor: 'rgba(0, 0, 0, 0.6)',
				zIndex: 2
			}}
		>
			<CircularProgress />
		</Box>);


	const boards = createTurnBoards(game.turns);

	const data = boards.filter((_, index) => index % 2 == 0)
		.map((board, index) => {
			const [emptyCount, whiteCount, blackCount] = countColors(board);

			return {
				name: `Turn: ${index * 2}`,
				uv: whiteCount - blackCount
			};
		});

	console.log(data);

	const gradientOffset = () => {
		const dataMax = Math.max(...data.map((i) => i.uv));
		const dataMin = Math.min(...data.map((i) => i.uv));

		if (dataMax <= 0) {
			return 0;
		}
		if (dataMin >= 0) {
			return 1;
		}

		return dataMax / (dataMax - dataMin);
	};

	const off = gradientOffset();

	const whitePlayer = game.players.find(pig => pig && pig.color === Color.white);
	const blackPlayer = game.players.find(pig => pig && pig.color === Color.black);

	const [emptyCount, whiteCount, blackCount] = countColors(game.board);

	return (
		<Box
			component="div"
			sx={{
				position: 'absolute',
				top: 0,
				bottom: 0,
				left: 0,
				right: 0,
				p: 10,
				backgroundColor: 'rgba(0, 0, 0, 0.6)',
				zIndex: 2
			}}
		>
			<Paper
				sx={{
					p: 4
				}}
			>
				<Typography variant="h3">
					Game Results
				</Typography>
				<Box
					component="div"
				>
					<Typography
						variant="h4"
						sx={{
							pt: 3
						}}
					>
						{game.name}
					</Typography>
					<Divider/>
					<Typography
						variant="body1"
						sx={{
							pt: 1,
							pb: 2
						}}
					>
						{game.description}
					</Typography>
					<Stack
						direction="row"
						spacing={2}
					>
						<Card
							variant="outlined"
							sx={{
								flex: 1
							}}
						>
							<CardContent
								sx={{
									':last-child': {
										pb: '0px'
									}
								}}
							>
								<Stack
									direction="column"
									alignItems="center"
									sx={{
										pb: 2
									}}
								>
									<Typography
										variant="h5"
										sx={{
											textAlign: 'center'
										}}
									>
										{whitePlayer!.player.username}
									</Typography>
									<Divider
										sx={{
											width: '100%',
											lineHeight: '30px'
										}}
									>
										<CircleIcon />
									</Divider>
									<Typography>
										{whiteCount}
									</Typography>
									<Typography

									>
										{
											whitePlayer!.result === GameResult.won
												? "Won"
												: whitePlayer!.result === GameResult.lost
													? "Lost"
													: "Draw"
										}
									</Typography>
								</Stack>
							</CardContent>
						</Card>
						<Card
							variant="outlined"
							sx={{
								flex: 1
							}}
						>
							<CardContent
								sx={{
									':last-child': {
										pb: '0px'
									}
								}}
							>
								<Stack
									direction="column"
									alignItems="center"
								>
									<Typography
										variant="h5"
										sx={{
											textAlign: 'center'
										}}
									>
										{blackPlayer!.player.username}
									</Typography>
									<Divider
										sx={{
											width: '100%',
											lineHeight: '30px'
										}}
									>
										<CircleOutlinedIcon />
									</Divider>
									<Typography>
										{blackCount}
									</Typography>
									<Typography

									>
										{
											blackPlayer!.result === GameResult.won
												? "Won"
												: blackPlayer!.result === GameResult.lost
													? "Lost"
													: "Draw"
										}
									</Typography>
								</Stack>
							</CardContent>
						</Card>
					</Stack>
					<ResponsiveContainer
						width="100%"
						minHeight="300px"
					>
						<AreaChart
							width={500}
							height={300}
							data={data}
							margin={{
								top: 0,
								right: 0,
								left: 0,
								bottom: 0,
							}}
						>
							<ReferenceLine y={0} stroke="white" />
							<XAxis dataKey="name" />
							<YAxis />
							<Tooltip />
							<defs>
								<linearGradient id="splitColor" x1="0" y1="0" x2="0" y2="1">
									<stop offset={off} stopColor="white" stopOpacity={1} />
									<stop offset={off} stopColor="black" stopOpacity={1} />
								</linearGradient>
							</defs>
							<Area type="monotone" dataKey="uv" stroke="#000" fill="url(#splitColor)" />
						</AreaChart>
					</ResponsiveContainer>
					<Button
						variant="contained"
						onClick={handleExit}
					>
						Exit
					</Button>
				</Box>
			</Paper>
		</Box>
	);
}

export default GameResults;