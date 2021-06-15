export default () => {

    const users = [];

    const userDetails = {
      name: "Jane Doe",
      email: "jane.doe@gmail.com",
      id: 1
    };
    
    for (let i = 0; i < 100000000; i++) {
      userDetails.id = i++;
      userDetails.dateJoined = Date.now();
    
      users.push(userDetails);
    }

    return users;

}