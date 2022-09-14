import { Box, Container, Paper, Stack, Typography } from '@mui/material';
import { FC, useEffect, useState } from 'react';
import { useParams } from 'react-router-dom';
import { getPlayer } from '../api';
import Player from '../model/Player';
import { useAppSelector } from '../store/Hooks';
import BadgeAvatar from './BadgeAvatar';

const Profile: FC = () => {
	const { username } = useParams();
	const auth = useAppSelector(state => state.auth);
	const [ player, setPlayer ] = useState<Player>()

	useEffect(() => {
		if (auth.token
			&& auth.token.length > 0
			&& username) {
			getPlayer(auth, username)
				.then((player) => {
					setPlayer(player);
				});
		}
	}, [auth.token, username]);

	return (
		<Box
			component="div"
			sx={{
				position: 'absolute',
				top: '500px',
				left: 0,
				right: 0
			}}
		>
			<Container>
				<Paper>
					<BadgeAvatar countryIso="nl" />
					<Stack direction="row">
						<Typography
							variant="h2"
						>
							{player?.username}
						</Typography>
						<Typography>
							{player?.amountWon}
						</Typography>
						<Typography>
							{player?.amountDraw}
						</Typography>
						<Typography>
							{player?.amountLost}
						</Typography>
					</Stack>
				</Paper>
			</Container>
		</Box>
	);
}

export default Profile;