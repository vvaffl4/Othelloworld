import { Box, Button, ButtonGroup, Container, FormControl, Grid, IconButton, InputAdornment, InputLabel, LinearProgress, MenuItem, OutlinedInput, Pagination, Paper, Select, SelectChangeEvent, Stack, Table, TableBody, TableCell, TableContainer, TableHead, TableRow, TextField, Typography } from '@mui/material';
import { FC, Suspense, useEffect, useState } from 'react';
import { useAppDispatch, useAppSelector } from '../store/Hooks';
import SearchIcon from '@mui/icons-material/Search';
import CancelIcon from '@mui/icons-material/Cancel';
import RefreshIcon from '@mui/icons-material/Refresh';
import AddIcon from '@mui/icons-material/Add';
import { fetchGames, searchGames } from '../store/Games';
import { useNavigate } from 'react-router-dom';
import GameListItem from './GameListItem';

const GameList: FC = () => {
	const dispatch = useAppDispatch();
	const navigate = useNavigate();

	const gameList = useAppSelector(state => state.games);
	const hasGame = useAppSelector(state => state.game.hasGame);

	const [searchValue, setSearchValue] = useState("");
	const [processing, isProcessing] = useState(true);
	const [selectedIndex, setSelectedIndex] = useState<number>();

	useEffect(() => {
		if (hasGame) {
			navigate('/play');
		}
	}, [hasGame]);

	useEffect(() => {
		isProcessing(false);
	}, [gameList]);

	useEffect(() => {
		dispatch(fetchGames(gameList.currentPage, gameList.pageSize));
	}, []);

	const handleSearchValueChange = (event: React.ChangeEvent<HTMLInputElement>) => {
		setSearchValue(event.target.value);
	}

	const handleClearSearchValue = () => {
		setSearchValue("");
		dispatch(fetchGames(gameList.currentPage, gameList.pageSize));
	}

	const handleSearch = () => {
		dispatch(searchGames(searchValue, gameList.currentPage, gameList.pageSize));
	}

	const handlePageSizeChange = (event: SelectChangeEvent<number>) => {
		if (searchValue.length > 0) {
			dispatch(searchGames(searchValue, gameList.currentPage, event.target.value as number));
		} else {
			dispatch(fetchGames(gameList.currentPage, event.target.value as number));
		}
	};

	const handleRefresh = () => {
		dispatch(fetchGames(gameList.currentPage, gameList.pageSize));
		isProcessing(true);
	}

	const handleCreate = () => {
		navigate('/play/new')
	}

	const handlePaginationChange = (_, value: number) => {
		if (searchValue.length > 0) {
			dispatch(searchGames(searchValue, value, gameList.pageSize));
		} else {
			dispatch(fetchGames(value, gameList.pageSize));
		}
	}

	return (
		<Box
			component="div"
			sx={{
				position: 'absolute',
				top: 0,
				left: 0,
				right: 0
			}}
		>
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
				<Paper sx={{
					mt: 16,
					overflow: 'hidden'
				}}>
					<div>
						<Box
							component="div"
							sx={{
								p: 4,
								backgroundColor: '#000000',
								borderBottom: '1px solid #ffffff17'
							}}
						>
							<div style={{ display: 'flex' }}>
								<div style={{ flexGrow: 1 }}>
									<Typography
										variant="h4"
									>
										Browse Games
									</Typography>
								</div>
								<div style={{ flexGrow: 0 }}>
									<FormControl
										variant="outlined"
										size="small"
										sx={{
											".MuiInputBase-root": {
												borderTopRightRadius: 0,
												borderBottomRightRadius: 0
											}
										}}
									>
										<InputLabel htmlFor="searchValue">Search</InputLabel>
										<OutlinedInput
											id="searchValue"
											value={searchValue}
											onChange={handleSearchValueChange}
											endAdornment={(
												searchValue.length > 0 && (
													<InputAdornment position="end">
														<IconButton
															aria-label="toggle password visibility"
															onClick={handleClearSearchValue}
															edge="end"
														>
															<CancelIcon />
														</IconButton>
													</InputAdornment>
											))}
											label="Password"
										/>
									</FormControl>
									<ButtonGroup
										variant="contained"
									>
										<Button
											endIcon={<SearchIcon />}
											sx={{
												height: '40px',
												borderRadius: 0
											}}
											onClick={handleSearch}
										>
											Search
										</Button>
										<Button
											endIcon={<RefreshIcon />}
											sx={{
												height: '40px'
											}}
											onClick={handleRefresh}
										>
											Refresh
										</Button>
										<Button
											endIcon={<AddIcon />}
											sx={{
												height: '40px'
											}}
											onClick={handleCreate}
										>
											Create
										</Button>
									</ButtonGroup>
								</div>
							</div>
						</Box>
						{ processing 
								? (
									<LinearProgress color="primary" />
								)
								: (
									<Stack
										direction="column"
										spacing={1}
										sx={{
											p: 2
										}}
									>
									{gameList.items.map((game, index) => {
											const handleItemClick = () => {
												setSelectedIndex(selectedIndex === index ? undefined : index);
											}

											return (
												<GameListItem
													key={index}
													game={game}
													expanded={selectedIndex === index}
													onClick={handleItemClick}
												/>
											)
										}
										)}
								</Stack>
							)
					}
					</div>
					<Stack
						direction="row"
						alignItems="flex-end"
						sx={{
							px: 2,
							pb: 2
						}}
					>
						<Pagination
							size="small"
							showFirstButton
							showLastButton
							count={gameList.totalPages}
							page={gameList.currentPage}
							onChange={handlePaginationChange}
						/>
						<FormControl
							variant="standard"
							sx={{
								mx: '35px',
								minWidth: '60px'
							}}
						>
							<InputLabel id="pageSize">Page Size</InputLabel>
							<Select
								labelId="pageSize"
								id="demo-simple-select"
								value={gameList.pageSize}
								label="Games Shown"
								onChange={handlePageSizeChange}
								sx={{
									textAlign: 'center'
								}}
							>
								<MenuItem value={5}>5</MenuItem>
								<MenuItem value={10}>10</MenuItem>
								<MenuItem value={20}>20</MenuItem>
							</Select>
						</FormControl>
					</Stack>
				</Paper>
				</Container>
			</Box>
	);
}

export default GameList;

