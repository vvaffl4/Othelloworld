import { Accordion, AccordionSummary, Box, Card, CardContent, Chip, Container, Divider, Paper, Stack, Typography } from '@mui/material';
import ExpandMoreIcon from '@mui/icons-material/ExpandMore';
import { FC, useEffect, useState } from 'react';
import { useParams } from 'react-router-dom';
import { getPlayer } from '../api';
import Player from '../model/Player';
import { useAppDispatch, useAppSelector } from '../store/Hooks';
import { selectAndFocusCountry } from '../store/World';
import ProfileGameHistoryItem from './ProfileGameHistoryItem';
import { GameResult } from '../model/PlayerInGame';
import ProfileGameHistory from './ProfileGameHistory';

const Profile: FC = () => {
	const dispatch = useAppDispatch();

	const { username } = useParams();
	const auth = useAppSelector(state => state.auth);
	const [ player, setPlayer ] = useState<Player>()

	useEffect(() => {
		if (auth.authenticated
			&& username) {
			getPlayer(auth, username)
				.then((player) => {
					setPlayer(player);

					dispatch(selectAndFocusCountry({
						isoCode: player.countryCode,
						altitude: 2,
						settings: {
							interactable: false,
							orbitControl: false,
							orbitAutoRotate: false,
							countrySelect: false,
							countrySelectMaxCount: 2
						}
					}))
				});
		}
	}, [auth.token, username]);

	return (
		<Box
			component="div"
			sx={{
				position: 'absolute',
				top: '550px',
				left: 0,
				right: 0
			}}
		>
			<Container>
				<Stack
					direction="column"
					justifyContent="center"
					alignItems="center"
					sx={{
						mb: '-20px'
					}}
				>
					{/*<Box*/}
					{/*	component="div"*/}
					{/*>*/}
					{/*	<BadgeAvatar countryIso={auth.player!.countryCode.toLowerCase()} />*/}
					{/*</Box>*/}
					<Paper
						sx={{
							width: '100%'
						}}
					>
						<Box
							component="div"
							sx={{
								backgroundColor: '#000000'
							}}
						>
							<Typography
								variant="h2"
								sx={{
									p: 2,
									textAlign: 'center'
								}}
							>
								{player && (player.username.substring(0, 1).toUpperCase() + player.username.substr(1))}
							</Typography>
						</Box>
						<Divider
							textAlign="center"
							sx={{
								mt: '-16px'
							}}
						>
							<Chip
								sx={{
									textAlign: 'center',
									fontSize: 24
								}}
								label={(player && player!.amountWon + player!.amountLost > 0)
									? `${(player!.amountWon / (player!.amountWon + player!.amountLost) * 100).toFixed(2)}%`
									: ''
								}
							/>
						</Divider>
						<Stack
							direction="row"
							alignItems="stretch"
							spacing={2}
							sx={{
								p: 2
							}}
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
									<Typography
										variant="h3"
										sx={{
											textAlign: 'center'
										}}
									>
										{player?.amountWon}
									</Typography>
									<Typography
										
									>
										Won
									</Typography>
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
									<Typography
										variant="h3"
										sx={{
											textAlign: 'center'
										}}
									>
										{player?.amountDraw}
									</Typography>
									<Typography

									>
										Draw
									</Typography>
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
									<Typography
										variant="h3"
										sx={{
											textAlign: 'center'
										}}
									>
										{player?.amountLost}
									</Typography>
									<Typography

									>
										Lost
									</Typography>
								</CardContent>
							</Card>
						</Stack>
						<ProfileGameHistory username={username} />
					</Paper>
				</Stack>
			</Container>
		</Box>
	);
}

export default Profile;