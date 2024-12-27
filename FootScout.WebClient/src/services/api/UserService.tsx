import axios from 'axios';
import ApiURL from '../../config/ApiConfig';
import AccountService from '../../services/api/AccountService';
import UserDTO from '../../models/dtos/UserDTO';
import UserUpdateDTO from '../../models/dtos/UserUpdateDTO';
import UserResetPasswordDTO from '../../models/dtos/UserResetPasswordDTO';
import ClubHistoryModel from '../../models/interfaces/ClubHistory';
import PlayerAdvertisement from '../../models/interfaces/PlayerAdvertisement';
import FavoritePlayerAdvertisement from '../../models/interfaces/FavoritePlayerAdvertisement';
import ClubOffer from '../../models/interfaces/ClubOffer';
import ClubAdvertisement from '../../models/interfaces/ClubAdvertisement';
import FavoriteClubAdvertisement from '../../models/interfaces/FavoriteClubAdvertisement';
import PlayerOffer from '../../models/interfaces/PlayerOffer';
import ChatModel from '../../models/interfaces/Chat';

const UserService = {
  async getUser(userId: string): Promise<UserDTO> {
    try {
      const authorizationHeader = await AccountService.getAuthorizationHeader();
      const response = await axios.get<UserDTO>(`${ApiURL}/users/${userId}`, {
        headers: {
          'Authorization': authorizationHeader
        }
      });
      return response.data;
    }
    catch (error) {
      if (axios.isAxiosError(error)) {
        console.error('Error fetching user, details:', error.response?.data || error.message);
      }
      else {
        console.error('Unexpected error:', error);
      }
      throw error;
    }
  },

  async getUsers(): Promise<UserDTO[]> {
    try {
      const authorizationHeader = await AccountService.getAuthorizationHeader();
      const response = await axios.get<UserDTO[]>(`${ApiURL}/users`, {
        headers: {
          'Authorization': authorizationHeader
        }
      });
      return response.data;
    }
    catch (error) {
      if (axios.isAxiosError(error)) {
        console.error('Error fetching users, details:', error.response?.data || error.message);
      }
      else {
        console.error('Unexpected error:', error);
      }
      throw error;
    }
  },

  async getOnlyUsers(): Promise<UserDTO[]> {
    try {
      const authorizationHeader = await AccountService.getAuthorizationHeader();
      const response = await axios.get<UserDTO[]>(`${ApiURL}/users/role/user`, {
        headers: {
          'Authorization': authorizationHeader
        }
      });
      return response.data;
    }
    catch (error) {
      if (axios.isAxiosError(error)) {
        console.error('Error fetching users from role user, details:', error.response?.data || error.message);
      }
      else {
        console.error('Unexpected error:', error);
      }
      throw error;
    }
  },

  async getOnlyAdmins(): Promise<UserDTO[]> {
    try {
      const authorizationHeader = await AccountService.getAuthorizationHeader();
      const response = await axios.get<UserDTO[]>(`${ApiURL}/users/role/admin`, {
        headers: {
          'Authorization': authorizationHeader
        }
      });
      return response.data;
    }
    catch (error) {
      if (axios.isAxiosError(error)) {
        console.error('Error fetching users from role admin, details:', error.response?.data || error.message);
      }
      else {
        console.error('Unexpected error:', error);
      }
      throw error;
    }
  },

  async getUserRole(userId: string): Promise<string> {
    try {
      const authorizationHeader = await AccountService.getAuthorizationHeader();
      const response = await axios.get<string>(`${ApiURL}/users/${userId}/role`, {
        headers: {
          'Authorization': authorizationHeader
        }
      });
      return response.data;
    }
    catch (error) {
      if (axios.isAxiosError(error)) {
        console.error('Error fetching user role, details:', error.response?.data || error.message);
      }
      else {
        console.error('Unexpected error:', error);
      }
      throw error;
    }
  },

  async getUserCount(): Promise<number> {
    try {
      const authorizationHeader = await AccountService.getAuthorizationHeader();
      const response = await axios.get<number>(`${ApiURL}/users/count`, {
        headers: {
          'Authorization': authorizationHeader
        }
      });
      return response.data;
    }
    catch (error) {
      if (axios.isAxiosError(error)) {
        console.error('Error fetching user count, details:', error.response?.data || error.message);
      }
      else {
        console.error('Unexpected error:', error);
      }
      throw error;
    }
  },

  async updateUser(userId: string, dto: UserUpdateDTO): Promise<void> {
    try {
      const authorizationHeader = await AccountService.getAuthorizationHeader();
      await axios.put(`${ApiURL}/users/${userId}`, dto, {
        headers: {
          'Authorization': authorizationHeader
        }
      });
    }
    catch (error) {
      if (axios.isAxiosError(error)) {
        console.error('Error updating user, details:', error.response?.data || error.message);
      }
      else {
        console.error('Unexpected error:', error);
      }
      throw error;
    }
  },

  async resetUserPassword(userId: string, dto: UserResetPasswordDTO): Promise<void> {
    try {
      const authorizationHeader = await AccountService.getAuthorizationHeader();
      await axios.put(`${ApiURL}/users/reset-password/${userId}`, dto, {
        headers: {
          'Authorization': authorizationHeader
        }
      });
    }
    catch (error) {
      if (axios.isAxiosError(error)) {
        console.error('Error reseting user\'s password, details:', error.response?.data || error.message);
      }
      else {
        console.error('Unexpected error:', error);
      }
      throw error;
    }
  },

  async deleteUser(userId: string): Promise<void> {
    try {
      const authorizationHeader = await AccountService.getAuthorizationHeader();
      await axios.delete(`${ApiURL}/users/${userId}`, {
        headers: {
          'Authorization': authorizationHeader
        }
      });
    }
    catch (error) {
      if (axios.isAxiosError(error)) {
        console.error('Error deleting user, details:', error.response?.data || error.message);
      }
      else {
        console.error('Unexpected error:', error);
      }
      throw error;
    }
  },

  async getUserClubHistory(userId: string): Promise<ClubHistoryModel[]> {
    try {
      const authorizationHeader = await AccountService.getAuthorizationHeader();
      const response = await axios.get<ClubHistoryModel[]>(`${ApiURL}/users/${userId}/club-history`, {
        headers: {
          'Authorization': authorizationHeader
        }
      });
      return response.data;
    }
    catch (error) {
      if (axios.isAxiosError(error)) {
        console.error("Error fetching user's club histories, details:", error.response?.data || error.message);
      }
      else {
        console.error('Unexpected error:', error);
      }
      throw error;
    }
  },

  async getUserPlayerAdvertisements(userId: string): Promise<PlayerAdvertisement[]> {
    try {
      const authorizationHeader = await AccountService.getAuthorizationHeader();
      const response = await axios.get<PlayerAdvertisement[]>(`${ApiURL}/users/${userId}/player-advertisements`, {
        headers: {
          'Authorization': authorizationHeader
        }
      });
      return response.data;
    }
    catch (error) {
      if (axios.isAxiosError(error)) {
        console.error("Error fetching user's player advertisements, details:", error.response?.data || error.message);
      }
      else {
        console.error('Unexpected error:', error);
      }
      throw error;
    }
  },

  async getUserActivePlayerAdvertisements(userId: string): Promise<PlayerAdvertisement[]> {
    try {
      const authorizationHeader = await AccountService.getAuthorizationHeader();
      const response = await axios.get<PlayerAdvertisement[]>(`${ApiURL}/users/${userId}/player-advertisements/active`, {
        headers: {
          'Authorization': authorizationHeader
        }
      });
      return response.data;
    }
    catch (error) {
      if (axios.isAxiosError(error)) {
        console.error("Error fetching user's active player advertisements, details:", error.response?.data || error.message);
      }
      else {
        console.error('Unexpected error:', error);
      }
      throw error;
    }
  },

  async getUserInactivePlayerAdvertisements(userId: string): Promise<PlayerAdvertisement[]> {
    try {
      const authorizationHeader = await AccountService.getAuthorizationHeader();
      const response = await axios.get<PlayerAdvertisement[]>(`${ApiURL}/users/${userId}/player-advertisements/inactive`, {
        headers: {
          'Authorization': authorizationHeader
        }
      });
      return response.data;
    }
    catch (error) {
      if (axios.isAxiosError(error)) {
        console.error("Error fetching user's inactive player advertisements, details:", error.response?.data || error.message);
      }
      else {
        console.error('Unexpected error:', error);
      }
      throw error;
    }
  },

  async getUserPlayerAdvertisementFavorites(userId: string): Promise<FavoritePlayerAdvertisement[]> {
    try {
      const authorizationHeader = await AccountService.getAuthorizationHeader();
      const response = await axios.get<FavoritePlayerAdvertisement[]>(`${ApiURL}/users/${userId}/player-advertisements/favorites`, {
        headers: {
          'Authorization': authorizationHeader
        }
      });
      return response.data;
    }
    catch (error) {
      if (axios.isAxiosError(error)) {
        console.error("Error fetching user's favorite player advertisements, details:", error.response?.data || error.message);
      }
      else {
        console.error('Unexpected error:', error);
      }
      throw error;
    }
  },

  async getUserActivePlayerAdvertisementFavorites(userId: string): Promise<FavoritePlayerAdvertisement[]> {
    try {
      const authorizationHeader = await AccountService.getAuthorizationHeader();
      const response = await axios.get<FavoritePlayerAdvertisement[]>(`${ApiURL}/users/${userId}/player-advertisements/favorites/active`, {
        headers: {
          'Authorization': authorizationHeader
        }
      });
      return response.data;
    }
    catch (error) {
      if (axios.isAxiosError(error)) {
        console.error("Error fetching user's active favorite player advertisements, details:", error.response?.data || error.message);
      }
      else {
        console.error('Unexpected error:', error);
      }
      throw error;
    }
  },

  async getUserInactivePlayerAdvertisementFavorites(userId: string): Promise<FavoritePlayerAdvertisement[]> {
    try {
      const authorizationHeader = await AccountService.getAuthorizationHeader();
      const response = await axios.get<FavoritePlayerAdvertisement[]>(`${ApiURL}/users/${userId}/player-advertisements/favorites/inactive`, {
        headers: {
          'Authorization': authorizationHeader
        }
      });
      return response.data;
    }
    catch (error) {
      if (axios.isAxiosError(error)) {
        console.error("Error fetching user's inactive favorite player advertisements, details:", error.response?.data || error.message);
      }
      else {
        console.error('Unexpected error:', error);
      }
      throw error;
    }
  },

  async getReceivedClubOffers(userId: string): Promise<ClubOffer[]> {
    try {
      const authorizationHeader = await AccountService.getAuthorizationHeader();
      const response = await axios.get<ClubOffer[]>(`${ApiURL}/users/${userId}/club-offers/received`, {
        headers: {
          'Authorization': authorizationHeader
        }
      });
      return response.data;
    }
    catch (error) {
      if (axios.isAxiosError(error)) {
        console.error("Error fetching user's received club offers, details:", error.response?.data || error.message);
      }
      else {
        console.error('Unexpected error:', error);
      }
      throw error;
    }
  },

  async getSentClubOffers(userId: string): Promise<ClubOffer[]> {
    try {
      const authorizationHeader = await AccountService.getAuthorizationHeader();
      const response = await axios.get<ClubOffer[]>(`${ApiURL}/users/${userId}/club-offers/sent`, {
        headers: {
          'Authorization': authorizationHeader
        }
      });
      return response.data;
    }
    catch (error) {
      if (axios.isAxiosError(error)) {
        console.error("Error fetching user's sent club offers, details:", error.response?.data || error.message);
      }
      else {
        console.error('Unexpected error:', error);
      }
      throw error;
    }
  },

  async getUserClubAdvertisements(userId: string): Promise<ClubAdvertisement[]> {
    try {
      const authorizationHeader = await AccountService.getAuthorizationHeader();
      const response = await axios.get<ClubAdvertisement[]>(`${ApiURL}/users/${userId}/club-advertisements`, {
        headers: {
          'Authorization': authorizationHeader
        }
      });
      return response.data;
    }
    catch (error) {
      if (axios.isAxiosError(error)) {
        console.error("Error fetching user's club advertisements, details:", error.response?.data || error.message);
      }
      else {
        console.error('Unexpected error:', error);
      }
      throw error;
    }
  },

  async getUserActiveClubAdvertisements(userId: string): Promise<ClubAdvertisement[]> {
    try {
      const authorizationHeader = await AccountService.getAuthorizationHeader();
      const response = await axios.get<ClubAdvertisement[]>(`${ApiURL}/users/${userId}/club-advertisements/active`, {
        headers: {
          'Authorization': authorizationHeader
        }
      });
      return response.data;
    }
    catch (error) {
      if (axios.isAxiosError(error)) {
        console.error("Error fetching user's active club advertisements, details:", error.response?.data || error.message);
      }
      else {
        console.error('Unexpected error:', error);
      }
      throw error;
    }
  },

  async getUserInactiveClubAdvertisements(userId: string): Promise<ClubAdvertisement[]> {
    try {
      const authorizationHeader = await AccountService.getAuthorizationHeader();
      const response = await axios.get<ClubAdvertisement[]>(`${ApiURL}/users/${userId}/club-advertisements/inactive`, {
        headers: {
          'Authorization': authorizationHeader
        }
      });
      return response.data;
    }
    catch (error) {
      if (axios.isAxiosError(error)) {
        console.error("Error fetching user's inactive club advertisements, details:", error.response?.data || error.message);
      }
      else {
        console.error('Unexpected error:', error);
      }
      throw error;
    }
  },

  async getUserClubAdvertisementFavorites(userId: string): Promise<FavoriteClubAdvertisement[]> {
    try {
      const authorizationHeader = await AccountService.getAuthorizationHeader();
      const response = await axios.get<FavoriteClubAdvertisement[]>(`${ApiURL}/users/${userId}/club-advertisements/favorites`, {
        headers: {
          'Authorization': authorizationHeader
        }
      });
      return response.data;
    }
    catch (error) {
      if (axios.isAxiosError(error)) {
        console.error("Error fetching user's favorite club advertisements, details:", error.response?.data || error.message);
      }
      else {
        console.error('Unexpected error:', error);
      }
      throw error;
    }
  },

  async getUserActiveClubAdvertisementFavorites(userId: string): Promise<FavoriteClubAdvertisement[]> {
    try {
      const authorizationHeader = await AccountService.getAuthorizationHeader();
      const response = await axios.get<FavoriteClubAdvertisement[]>(`${ApiURL}/users/${userId}/club-advertisements/favorites/active`, {
        headers: {
          'Authorization': authorizationHeader
        }
      });
      return response.data;
    }
    catch (error) {
      if (axios.isAxiosError(error)) {
        console.error("Error fetching user's active favorite club advertisements, details:", error.response?.data || error.message);
      }
      else {
        console.error('Unexpected error:', error);
      }
      throw error;
    }
  },

  async getUserInactiveClubAdvertisementFavorites(userId: string): Promise<FavoriteClubAdvertisement[]> {
    try {
      const authorizationHeader = await AccountService.getAuthorizationHeader();
      const response = await axios.get<FavoriteClubAdvertisement[]>(`${ApiURL}/users/${userId}/club-advertisements/favorites/inactive`, {
        headers: {
          'Authorization': authorizationHeader
        }
      });
      return response.data;
    }
    catch (error) {
      if (axios.isAxiosError(error)) {
        console.error("Error fetching user's inactive favorite club advertisements, details:", error.response?.data || error.message);
      }
      else {
        console.error('Unexpected error:', error);
      }
      throw error;
    }
  },

  async getReceivedPlayerOffers(userId: string): Promise<PlayerOffer[]> {
    try {
      const authorizationHeader = await AccountService.getAuthorizationHeader();
      const response = await axios.get<PlayerOffer[]>(`${ApiURL}/users/${userId}/player-offers/received`, {
        headers: {
          'Authorization': authorizationHeader
        }
      });
      return response.data;
    }
    catch (error) {
      if (axios.isAxiosError(error)) {
        console.error("Error fetching user's received player offers, details:", error.response?.data || error.message);
      }
      else {
        console.error('Unexpected error:', error);
      }
      throw error;
    }
  },

  async getSentPlayerOffers(userId: string): Promise<PlayerOffer[]> {
    try {
      const authorizationHeader = await AccountService.getAuthorizationHeader();
      const response = await axios.get<PlayerOffer[]>(`${ApiURL}/users/${userId}/player-offers/sent`, {
        headers: {
          'Authorization': authorizationHeader
        }
      });
      return response.data;
    }
    catch (error) {
      if (axios.isAxiosError(error)) {
        console.error("Error fetching user's sent player offers, details:", error.response?.data || error.message);
      }
      else {
        console.error('Unexpected error:', error);
      }
      throw error;
    }
  },

  async getUserChats(userId: string): Promise<ChatModel[]> {
    try {
      const authorizationHeader = await AccountService.getAuthorizationHeader();
      const response = await axios.get<ChatModel[]>(`${ApiURL}/users/${userId}/chats`, {
        headers: {
          'Authorization': authorizationHeader
        }
      });
      return response.data;
    }
    catch (error) {
      if (axios.isAxiosError(error)) {
        console.error("Error fetching user's chats, details:", error.response?.data || error.message);
      }
      else {
        console.error('Unexpected error:', error);
      }
      throw error;
    }
  },

  async exportUsersToCsv(): Promise<void> {
    try {
      const authorizationHeader = await AccountService.getAuthorizationHeader();

      const response = await axios.get(`${ApiURL}/users/export`, {
        headers: {
          'Authorization': authorizationHeader
        },
        responseType: 'blob'
      });

      const url = window.URL.createObjectURL(new Blob([response.data], { type: 'text/csv' }));
      const link = document.createElement('a');
      link.href = url;
      link.setAttribute('download', 'users.csv');

      document.body.appendChild(link);
      link.click();
      link.remove();
    }
    catch (error) {
      if (axios.isAxiosError(error)) {
        console.error("Error exporting users to CSV, details:", error.response?.data || error.message);
      }
      else {
        console.error('Unexpected error:', error);
      }
      throw error;
    }
  }
};

export default UserService;