import { Container, Grid, Paper, Typography } from '@mui/material';
import { FC } from 'react';
import HomeBanner from './HomeBanner';
import HomeCardList from './HomeCardList';

const Home: FC = () => {

	return (
		<Container>
			<Typography
				variant="h1"
				color="white"
				sx={{
					pt: 36,
					fontWeight: 700,
					textAlign: 'right'
				}}
			>
				OTHELLO
			</Typography>
			<Typography
				variant="h2"
				color={ '#f06292' }
				sx={{
					textAlign: 'right'
				}}
			>
				WORLD
			</Typography>
			<Paper
				sx={{
					mt: 16,
					overflow: 'hidden'
				}}
			>
				<HomeBanner />
				<HomeCardList />
				<Typography
					variant="body1"
					sx={{
						m: 2,
						p: 2,
						backgroundColor: '#121212',
						borderRadius: '5px',
						fontSize: '16px'
					}}
				>
					<b>Reversi (Othello)</b> is a strategy board game for two players, played on an 8×8 uncheckered board. There are sixty-four identical game pieces called disks (often spelled "discs"), which are light on one side and dark on the other. Players take turns placing disks on the board with their assigned color facing up. During a play, any disks of the opponent's color that are in a straight line and bounded by the disk just placed and another disk of the current player's color are turned over to the current player's color. The object of the game is to have the majority of disks turned to display your color when the last playable empty square is filled.
				</Typography>
			</Paper>
		</Container>
	)
}

export default Home;