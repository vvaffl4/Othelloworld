import { Box, Button, Card, CardActions, CardContent, Container, Paper, Stack, Typography } from "@mui/material";
import { FC } from "react";

const GameOption: FC = () => {
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
				color={'#f06292'}
				sx={{
					textAlign: 'right'
				}}
			>
				WORLD
			</Typography>
			<Paper
				elevation={4}
				sx={{
					mt: 16
				}}
			>
				<Box
					component="div"
					sx={{
						p: 4,
						backgroundColor: '#000000',
						borderBottom: '1px solid #ffffff17'
					}}
				>
					<Typography
						variant="h3"
						color='white'
					>
						Play
					</Typography>
				</Box>
				<Stack
					direction="row"
					spacing={4}
					sx={{
						py: 4,
						px: 4
					}}
				>
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
					<Card variant="outlined">
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
				</Stack>
			</Paper>
		</Container>
	);
}

export default GameOption;