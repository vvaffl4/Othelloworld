﻿import { Divider, Stack, Typography } from '@mui/material';
import { FC, useEffect, useState } from 'react';
import { getGameHistory } from '../api';
import Game from '../model/Game';
import { countColors } from '../services/BoardService';
import { useAppSelector } from '../store/Hooks';
import ProfileGameHistoryItem from './ProfileGameHistoryItem';

interface ProfileGameHistoryProps {
	username?: string;
}

const ProfileGameHistory: FC<ProfileGameHistoryProps> = ({ username }) => {
	const [games, setGames] = useState<Game[]>([]);
	const [selectedIndex, setSelectedIndex] = useState<number | undefined>(undefined);

	const auth = useAppSelector(state => state.auth);

	useEffect(() => {

		if (auth.authenticated
		 && username) {
			getGameHistory(auth, username)
				.then(setGames);
		}
	}, [auth]);


	return (
		<>
			<Divider textAlign="center">
				<Typography
					variant="h6"
				>
					Game History
				</Typography>
			</Divider>
			<Stack
				direction="column"
				spacing={1}
				sx={{
					p: 2
				}}
			>
				{games.map((game, index) => {
					const handleItemClick = () => {
						setSelectedIndex(selectedIndex === index ? undefined : index);
					}

					const userPlayer = game.players.find(pig => pig?.player.username === username)!;
					const opponentPlayer = game.players.find(pig => pig?.player.username !== username);

					const [emptyCount, whiteCount, blackCount] = countColors(game.board);

					return (
						<ProfileGameHistoryItem
							key={index}
							expanded={selectedIndex === index}
							result={userPlayer.result}
							playerOne={userPlayer.player.username}
							playerTwo={opponentPlayer ? opponentPlayer.player.username: ''}
							whiteCount={whiteCount}
							blackCount={blackCount}
							onClick={handleItemClick}
						/>
					)}
				)}
			</Stack>
		</>
	);
}

export default ProfileGameHistory;